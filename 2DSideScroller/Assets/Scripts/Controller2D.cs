using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour
{
    // Set layer, so no objects collide with themselves
    public LayerMask collisionMask;

    // skinwidth is a small part of the player model on the bottom
    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    float climbAngle = 80;
    float descendAngle = 75;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D boxcollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Start()
    {
        boxcollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    // Calculate exact position of object
    void UpdateRaycastOrigins()
    {
        Bounds bounds = boxcollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    void CalculateRaySpacing()
    {
        Bounds bounds = boxcollider.bounds;
        bounds.Expand(skinWidth * -2);

        // make sure that there are at least 2 rays
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    // Ray casts prevent an object from going through other objects (aka collision handling)
    public void Move(Vector2 moveDistance)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.moveDistanceOld = moveDistance;

        if (moveDistance.y < 0)
            Descend(ref moveDistance);
        if (moveDistance.x != 0)
            HorizontalCollisions(ref moveDistance);
        if (moveDistance.y != 0)
            VerticalCollisions(ref moveDistance);

        transform.Translate(moveDistance);
    }

    void HorizontalCollisions(ref Vector2 moveDistance)
    {
        float directionX = Mathf.Sign(moveDistance.x);
        float rayLength = Mathf.Abs(moveDistance.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                float angle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && angle <= climbAngle)
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
                    Climb(ref moveDistance, angle);
                    moveDistance.x += distanceToSlope * directionX;
                }

                if (!collisions.climbing || angle > climbAngle)
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

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
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
                }
            }
        }
    }

    void Climb(ref Vector2 moveDistance, float angle)
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
        }
    }

    void Descend(ref Vector2 moveDistance)
    {
        float directionX = Mathf.Sign(moveDistance.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle != 0 && angle <= descendAngle)
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
                    }
                }
            }
        }
    }

    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbing;
        public bool descending;
        public float angle, angleOld;
        public Vector2 moveDistanceOld;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbing = false;
            descending = false;

            angleOld = angle;
            angle = 0;
        }
    }
}
