using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region properties

    public float health = 100;
    public float maxJumpHeight = 4;
    public float minJumpHeight = 0.5f;
    public float timeToMaxJumpHeight = .4f;
    float maxVelocityTimeAir = .2f;
    float maxVelocityTimeGround = .1f;
    float moveSpeed = 6;

    #endregion

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector2 velocity;
    float velocityXSmoothing;

    Controller2D controller;
    Vector2 directionalInput;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToMaxJumpHeight, 2);
        maxJumpVelocity = (2 * maxJumpHeight) / timeToMaxJumpHeight;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
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
    }

    public void Die()
    {
        health = 0;
        print(health);
        Respawn();
    }

    void Respawn()
    {
        health = 100;
        Vector3 spawnPoint = GameObject.Find("SpawnPoint").GetComponent<SpawnPoint>().spawnPoint;
        transform.position = spawnPoint;
        print(health);
    }
}
