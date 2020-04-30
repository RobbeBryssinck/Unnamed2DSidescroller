using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{
    #region properties

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

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToMaxJumpHeight, 2);
        maxJumpVelocity = (2 * maxJumpHeight) / timeToMaxJumpHeight;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = maxJumpVelocity;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (velocity.y > minJumpVelocity) // prevents acceleration mid-air
            {
                velocity.y = minJumpVelocity;
            }
        }

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? maxVelocityTimeGround : maxVelocityTimeAir);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
    }
}
