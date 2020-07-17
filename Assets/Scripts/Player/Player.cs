using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region properties

    public float Health { get; private set; }
    public float VelocityY { get { return velocity.y; } set { velocity.y = value; }  }
    
    public float initialHealth = 300f;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 0.5f;
    public float timeToMaxJumpHeight = .4f;
    float maxVelocityTimeAir = .2f;
    float maxVelocityTimeGround = .1f;
    float moveSpeed = 6;

    // These fields set the offset of the player's box collider when turning
    // Given an empty Vector2 here so the compiler stops complaining
    [SerializeField]
    private Vector2 bcRight = new Vector2();
    [SerializeField]
    private Vector2 bcLeft = new Vector2();

    #endregion

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;
    [HideInInspector]
    public bool isHit;

    Controller2D controller;
    SpawnPoint spawnPoint;
    Vector2 directionalInput;
    PlayerUIController playerUIController;
    private FSMSystem fsm;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    void Start()
    {
        isHit = false;

        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToMaxJumpHeight, 2);
        maxJumpVelocity = (2 * maxJumpHeight) / timeToMaxJumpHeight;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        Health = initialHealth;

        spawnPoint = GameObject.Find("SpawnPoint").GetComponent<SpawnPoint>();
        transform.position = spawnPoint.transform.position;
        playerUIController = GameObject.Find("PlayerUI").GetComponent<PlayerUIController>();

        MakeFSM();
    }

    private void MakeFSM()
    {
        PlayerCombatState playerCombat = new PlayerCombatState(gameObject);
        playerCombat.AddTransition(Transition.PlayerHit, StateID.Invulnerable);

        InvulnerableState invulnerable = new InvulnerableState(1.0f, gameObject);
        invulnerable.AddTransition(Transition.DoneInvulnerable, StateID.PlayerCombat);

        fsm = new FSMSystem();
        fsm.AddState(playerCombat);
        fsm.AddState(invulnerable);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
        if (input == Vector2.right)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            GetComponent<BoxCollider2D>().offset = bcRight;
        }
        if (input == Vector2.left)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            GetComponent<BoxCollider2D>().offset = bcLeft;
        }
    }

    public void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) // not jumping against max slope
                {
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
                velocity.y = maxJumpVelocity;
            }
        }
    }

    public void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity) // prevents acceleration mid-air
        {
            velocity.y = minJumpVelocity;
        }
    }

    void Update()
    {
        fsm.CurrentState.Reason(gameObject, null);
        fsm.CurrentState.Act(gameObject, null);

        CalculateVelocity();

        controller.Move(velocity * Time.deltaTime, directionalInput);

        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            else
                velocity.y = 0;
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? maxVelocityTimeGround : maxVelocityTimeAir);
        velocity.y += gravity * Time.deltaTime;

        if (controller.collisions.hitEnemyOnTop)
            velocity.y = 15;
    }

    public void HandleHit(float damage)
    {
        if (!isHit)
        {
            isHit = true;
            TakeDamage(damage);
        }
    }

    private void TakeDamage(float damage)
    {
        Health -= damage;
        playerUIController.TakeDamage(damage);

        if (Health <= 0f)
            Die();
    }

    public void Die()
    {
        Respawn();
    }

    void Respawn()
    {
        playerUIController.RegenerateHealth();
        transform.position = spawnPoint.transform.position;
        Health = initialHealth;
    }
}
