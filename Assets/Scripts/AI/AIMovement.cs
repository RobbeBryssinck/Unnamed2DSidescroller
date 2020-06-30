using UnityEngine;

public class AIMovement : MonoBehaviour
{
    // Set layer, so no objects collide with themselves
    public LayerMask collisionMask;

    private RaycastController rcController;

    [SerializeField]
    private float moveSpeed = 2f;

    private float gravity;

    private Vector2 velocity;
    private Vector2 moveDistance;
    private Vector3 destination;
    private Collisions collisions;

    private void Start()
    {
        rcController = new RaycastController(GetComponent<BoxCollider2D>());

        gravity = -10f;
    }

    public void Move(Vector3 destination)
    {
        this.destination = destination;

        rcController.UpdateRaycastOrigins();
        collisions.Reset();

        CalculateVelocity();
        CalculateHorizontalMovement();
        CalculateVerticalMovement();
        transform.Translate(moveDistance);

        if (collisions.above || collisions.below)
            velocity.y = 0;

        if (Mathf.Sign(moveDistance.x) == 1)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }

    private void CalculateVelocity()
    {
        float directionX = Mathf.Sign(destination.x - transform.position.x);
        moveDistance.x = directionX * moveSpeed * Time.deltaTime;

        velocity.y += gravity * Time.deltaTime;
        moveDistance.y = velocity.y * Time.deltaTime;
    }

    // TODO: put destination in calculate parameters
    private void CalculateHorizontalMovement()
    {
        float directionX = Mathf.Sign(destination.x - transform.position.x);
        float rayLength = Mathf.Abs(moveDistance.x) + RaycastController.skinWidth;

        for (int i = 0; i < rcController.horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rcController.raycastOrigins.bottomLeft : rcController.raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * rcController.horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                moveDistance.x = (hit.distance - RaycastController.skinWidth) * directionX;
                rayLength = hit.distance;
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

        public void Reset()
        {
            below = false;
            above = false;
        }
    }
}
