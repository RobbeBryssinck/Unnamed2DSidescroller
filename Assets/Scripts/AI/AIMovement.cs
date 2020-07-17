using UnityEngine;

public class AIMovement : MonoBehaviour
{
    // Set layer, so no objects collide with themselves
    public LayerMask collisionMask;

    private RaycastController rcController;

    [SerializeField]
    private float moveSpeed = 2f;
    [SerializeField]
    private float jumpHeight = 4.0f;
    [SerializeField]
    private float timeToJumpHeight = 0.4f;

    public float DirectionX { get; set; } = -1f;

    private float gravity;
    private float jumpVelocity;

    private Vector2 velocity;
    public Collisions collisions;

    private void Start()
    {
        rcController = new RaycastController(GetComponent<BoxCollider2D>());

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpHeight, 2);
        jumpVelocity = (2 * jumpHeight) / timeToJumpHeight;
    }

    public void Move(Vector2 moveDistance)
    {
        rcController.UpdateRaycastOrigins();
        collisions.Reset();

        CalculateHorizontalMovement(ref moveDistance);
        CalculateVerticalMovement(ref moveDistance);
        transform.Translate(moveDistance);

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

    private void CalculateHorizontalMovement(ref Vector2 moveDistance)
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
                    collisions.touchedPlayerHorizontally = true;

                collisions.left = DirectionX == -1;
                collisions.right = DirectionX == 1;
            }
        }
    }

    private void CalculateVerticalMovement(ref Vector2 moveDistance)
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

        velocity = CalculateVelocity();
        Vector2 moveDistance = velocity * Time.deltaTime;
        CalculateVerticalMovement(ref moveDistance);
        moveDistance.x = 0;
        transform.Translate(moveDistance);

        if (collisions.above || collisions.below)
            velocity.y = 0;
    }

    public struct Collisions
    {
        public bool below, above;
        public bool left, right;
        public bool touchedPlayerHorizontally;

        public void Reset()
        {
            below = above = false;
            left = right = false;
            touchedPlayerHorizontally = false;
        }
    }
}
