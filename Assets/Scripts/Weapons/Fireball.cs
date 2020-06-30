using UnityEngine;

public class Fireball : MonoBehaviour
{
    // Set layer, so no objects collide with themselves
    public LayerMask collisionMask;

    private RaycastController rcController;

    public float speed;
    public float newScale;
    public int direction;
    public float damage;

    private Vector2 moveDistance;
    private CollisionInfo collisions;

    protected void Start()
    {
        rcController = new RaycastController(GetComponent<BoxCollider2D>());
        SetScale(newScale);
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
                        enemy.TakeDamage(damage);
                }

                if (hit.collider.tag == "Player")
                {
                    Player player = hit.collider.GetComponent<Player>();
                    if (player != null)
                        player.TakeDamage(damage);
                }

                Destruct();
                break;
            }
        }
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
