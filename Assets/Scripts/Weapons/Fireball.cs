using UnityEngine;

public class Fireball : Weapon
{
    public float speed;
    public float newScale;
    public int direction;

    private Vector2 moveDistance;
    private CollisionInfo collisions;

    protected void Start()
    {
        SetScale(newScale);
        rcController = new RaycastController(GetComponent<BoxCollider2D>());
    }

    private void SetScale(float newScale)
    {
        transform.parent = null;
        transform.localScale = Vector2.one * newScale;
    }

    private void Update()
    {
        rcController.UpdateRaycastOrigins();
        collisions.Reset();
        moveDistance = Vector2.right * direction * speed * Time.deltaTime;
        HorizontalCollisions(ref moveDistance);
        transform.Translate(moveDistance);
    }

    private void Destruct()
    {
        Destroy(gameObject);
    }

    private void HorizontalCollisions(ref Vector2 moveDistance)
    {
        float directionX = Mathf.Sign(moveDistance.x);
        float rayLength = Mathf.Abs(moveDistance.x);

        for (int i = 0; i < rcController.horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? rcController.raycastOrigins.bottomLeft : rcController.raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * rcController.horizontalRaySpacing * i;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Enemy")
                {
                    NPCController enemy = hit.collider.GetComponent<NPCController>();
                    if (enemy != null)
                        enemy.HandleHit(damage);
                }

                if (hit.collider.tag == "Player")
                {
                    Player player = hit.collider.GetComponent<Player>();
                    if (player != null)
                        player.HandleHit(damage);
                }

                Destruct();
                break;
            }
        }
    }

    public override void HandleHit()
    {
        Destruct();
    }

    private struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
