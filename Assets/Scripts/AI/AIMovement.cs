﻿using UnityEngine;

public class AIMovement : MonoBehaviour
{
    // Set layer, so no objects collide with themselves
    public LayerMask collisionMask;

    private RaycastController rcController;

    [SerializeField]
    private float moveSpeed = 2f;
    private float jumpHeight = 4.0f;
    private float timeToJumpHeight = 0.4f;

    public float DirectionX { get; set; } = -1f;

    private float gravity;
    private float jumpVelocity;

    private Vector2 velocity;
    private Vector2 moveDistance;
    public Collisions collisions;

    private void Start()
    {
        rcController = new RaycastController(GetComponent<BoxCollider2D>());

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpHeight, 2);
        jumpVelocity = (2 * jumpHeight) / timeToJumpHeight;
    }

    public void Move(Vector3 moveDistance)
    {
        this.moveDistance = moveDistance;

        rcController.UpdateRaycastOrigins();
        collisions.Reset();

        CalculateHorizontalMovement();
        CalculateVerticalMovement();
        transform.Translate(this.moveDistance);

        if (collisions.above || collisions.below)
            velocity.y = 0;

        if (Mathf.Sign(moveDistance.x) == 1)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }

    public Vector2 CalculateVelocity()
    {
        velocity.x = moveSpeed * DirectionX;
        velocity.y += gravity * Time.deltaTime;
        return velocity;
    }

    public Vector2 CalculateJumpVelocity()
    {
        velocity.y = jumpVelocity;
        return velocity;
    }

    public void SetDirection(Vector2 destination)
    {
        DirectionX = Mathf.Sign(destination.x - transform.position.x);
    }

    // TODO: put destination in calculate parameters
    private void CalculateHorizontalMovement()
    {
        float rayLength = Mathf.Abs(moveDistance.x) + RaycastController.skinWidth;

        for (int i = 0; i < rcController.horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (DirectionX == -1) ? rcController.raycastOrigins.bottomLeft : rcController.raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * rcController.horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * DirectionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * DirectionX, Color.red);

            if (hit)
            {
                moveDistance.x = (hit.distance - RaycastController.skinWidth) * DirectionX;
                rayLength = hit.distance;

                if (hit.collider.tag == "Player")
                {
                    collisions.touchedPlayerHorizontally = true;
                }
            }
        }
    }

    private void CalculateVerticalMovement()
    {
        float directionY = Mathf.Sign(moveDistance.y);
        float rayLength = Mathf.Abs(moveDistance.y) + RaycastController.skinWidth;

        for (int i = 0; i < rcController.verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? rcController.raycastOrigins.bottomLeft : rcController.raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (rcController.verticalRaySpacing * i + moveDistance.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                moveDistance.y = (hit.distance - RaycastController.skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }
    }

    public void SimulateGravity()
    {
        rcController.UpdateRaycastOrigins();
        collisions.Reset();

        CalculateVelocity();
        CalculateVerticalMovement();
        moveDistance.x = 0;
        transform.Translate(moveDistance);

        if (collisions.above || collisions.below)
            velocity.y = 0;
    }

    public struct Collisions
    {
        public bool below, above;
        public bool touchedPlayerHorizontally;

        public void Reset()
        {
            below = false;
            above = false;
            touchedPlayerHorizontally = false;
        }
    }
}
