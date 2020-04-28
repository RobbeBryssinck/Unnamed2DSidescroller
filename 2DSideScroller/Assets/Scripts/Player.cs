using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Controller2D))]
public class Player: MonoBehaviour
{
    #region properties

    public float jumpHeight = 4;
    public float timeToMaxJumpHeight = .4f;
    float maxVelocityTimeAir = .2f;
    float maxVelocityTimeGround = .1f;
    float moveSpeed = 6;

    #endregion

    float gravity;
    float jumpVelocity;
    Vector2 moveDistance;
    float velocityXSmoothing;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToMaxJumpHeight, 2);
        jumpVelocity = (2 * jumpHeight) / timeToMaxJumpHeight;
    }

    void Update()
    {
        if (controller.collisions.above || controller.collisions.below)
        {
            moveDistance.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            moveDistance.y = jumpVelocity;
        }

        float targetVelocityX = input.x * moveSpeed;
        moveDistance.x = Mathf.SmoothDamp(moveDistance.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? maxVelocityTimeGround : maxVelocityTimeAir);
        moveDistance.y += gravity * Time.deltaTime;
        controller.Move(moveDistance * Time.deltaTime);
    }
}
