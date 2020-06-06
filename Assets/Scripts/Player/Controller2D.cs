﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2D : RaycastController
{
    public float maxSlopeAngle = 80;

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 playerInput;
    Player player;

    protected override void Start()
    {
        base.Start();
        player = GetComponent<Player>();
    }

    // Ray casts prevent an object from going through other objects (aka collision handling)
    public void Move(Vector2 moveDistance, bool standingOnPlatform = false)
    {
        Move(moveDistance, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector2 moveDistance, Vector2 input, bool standingOnPlatform = false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveDistanceOld = moveDistance;
        playerInput = input;

        if (moveDistance.y < 0)
            Descend(ref moveDistance);
        if (moveDistance.x != 0)
            HorizontalCollisions(ref moveDistance);
        if (moveDistance.y != 0)
            VerticalCollisions(ref moveDistance);

        transform.Translate(moveDistance);

        if (standingOnPlatform == true)
            collisions.below = true;
    }

    void HorizontalCollisions(ref Vector2 moveDistance)
    {
        float directionX = Mathf.Sign(moveDistance.x);
        float rayLength = Mathf.Abs(moveDistance.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "DeathZone" || hit.collider.tag == "Enemy")
                {
                    collisions.isDamagedByEnemy = true;
                    player.Die();
                    break;
                }
                // fix movement bug when inside other object
                if (hit.distance == 0)
                {
                    continue;
                }

                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && angle <= maxSlopeAngle)
                {
                    if (collisions.descending)
                    {
                        collisions.descending = false;
                        moveDistance = collisions.moveDistanceOld;
                    }
                    float distanceToSlope = 0;
                    if (angle != collisions.angleOld)
                    {
                        distanceToSlope = hit.distance - skinWidth;
                        moveDistance.x -= distanceToSlope * directionX;
                    }
                    Climb(ref moveDistance, angle, hit.normal);
                    moveDistance.x += distanceToSlope * directionX;
                }

                if (!collisions.climbing || angle > maxSlopeAngle)
                {
                    moveDistance.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    // fixes bug where player stutters while climbing and pressing against wall
                    if (collisions.climbing)
                    {
                        moveDistance.y = Mathf.Tan(collisions.angle * Mathf.Deg2Rad) * Mathf.Abs(moveDistance.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void VerticalCollisions(ref Vector2 moveDistance)
    {
        float directionY = Mathf.Sign(moveDistance.y);
        float rayLength = Mathf.Abs(moveDistance.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveDistance.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "DeathZone")
                {
                    player.Die();
                    break;
                }
                if (hit.collider.tag == "Enemy" && directionY == -1 && collisions.isDamagedByEnemy == false)
                {
                    Enemy enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        // TODO: Move damage value.
                        enemy.TakeDamage(100);
                    }
                    collisions.killedEnemy = true;
                }
                else
                {
                    if (hit.collider.tag == "Through")
                    {
                        if (directionY == 1 || hit.distance == 0)
                            continue;
                        if (collisions.fallingThroughPlatform)
                        {
                            continue;
                        }
                        if (playerInput.y == -1)
                        {
                            collisions.fallingThroughPlatform = true;
                            Invoke("ResetFallingThroughPlatform", .5f);
                            continue;
                        }
                    }

                    moveDistance.y = (hit.distance - skinWidth) * directionY;
                    rayLength = hit.distance;

                    // fixes bug where player stutters while climbing and pressing against ceiling
                    if (collisions.climbing)
                    {
                        moveDistance.x = moveDistance.y / Mathf.Tan(collisions.angle * Mathf.Deg2Rad) * Mathf.Sign(moveDistance.x);
                    }

                    collisions.below = directionY == -1;
                    collisions.above = directionY == 1;
                }
            }
        }

        if (collisions.climbing)
        {
            float directionX = Mathf.Sign(moveDistance.x);
            rayLength = Mathf.Abs(moveDistance.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveDistance.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle != collisions.angle)
                {
                    moveDistance.x = (hit.distance - skinWidth) * directionX;
                    collisions.angle = angle;
                    collisions.slopeNormal = hit.normal;
                }
            }
        }
    }

    void Climb(ref Vector2 moveDistance, float angle, Vector2 slopeNormal)
    {
        float climbDistance = Mathf.Abs(moveDistance.x);
        float climbDistanceY = Mathf.Sin(angle * Mathf.Deg2Rad) * climbDistance;

        if (moveDistance.y <= climbDistanceY)
        {
            moveDistance.x = Mathf.Cos(angle * Mathf.Deg2Rad) * climbDistance * Mathf.Sign(moveDistance.x);
            moveDistance.y = climbDistanceY;
            collisions.angle = angle;
            collisions.climbing = true;
            collisions.below = true;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void Descend(ref Vector2 moveDistance)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveDistance.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveDistance.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveDistance);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveDistance);
        }

        if (!collisions.slidingDownMaxSlope)
        {

            float directionX = Mathf.Sign(moveDistance.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (angle != 0 && angle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Abs(moveDistance.x))
                        {
                            float descendDistance = Mathf.Abs(moveDistance.x);
                            float descendDistanceY = Mathf.Sin(angle * Mathf.Deg2Rad) * descendDistance;
                            moveDistance.x = Mathf.Cos(angle * Mathf.Deg2Rad) * descendDistance * Mathf.Sign(moveDistance.x);
                            moveDistance.y -= descendDistanceY;

                            collisions.angle = angle;
                            collisions.descending = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }
                    }
                }
            }
        }
    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveDistance)
    {
        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle > maxSlopeAngle)
            {
                moveDistance.x = hit.normal.x * (Mathf.Abs(moveDistance.y) - hit.distance) / Mathf.Tan(angle * Mathf.Deg2Rad);

                collisions.angle = angle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetFallingThroughPlatform()
    {
        collisions.fallingThroughPlatform = false;
    }

    void PlayerDie()
    {
        player.Die();
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbing;
        public bool descending;
        public bool slidingDownMaxSlope;

        public float angle, angleOld;
        public Vector2 slopeNormal;
        public Vector2 moveDistanceOld;
        public bool fallingThroughPlatform;

        public bool killedEnemy;
        // TODO: fix this suboptimal solution
        public bool isDamagedByEnemy;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbing = false;
            descending = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            angleOld = angle;
            angle = 0;

            killedEnemy = false;
            isDamagedByEnemy = false;
        }
    }
}