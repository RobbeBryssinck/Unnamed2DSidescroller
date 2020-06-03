using UnityEngine;

public class MedievalSwordsman : Enemy
{
    protected override float Health { get; set; } = 100;

    [SerializeField]
    protected Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    [SerializeField]
    protected float moveSpeed;
    int fromWaypointIndex;
    float percentBetweenWaypoints;
    Vector3 newPos;

    public virtual void Start()
    {

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    public virtual void Update()
    {
        Patrol();
    }

    public virtual void Patrol()
    {
        int toWaypointIndex = fromWaypointIndex + 1;
        percentBetweenWaypoints += Time.deltaTime * moveSpeed;

        newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (fromWaypointIndex >= globalWaypoints.Length - 1)
            {
                fromWaypointIndex = 0;
                System.Array.Reverse(globalWaypoints);
            }
        }

        newPos -= transform.position;
        transform.Translate(newPos);

        if (Mathf.Sign(newPos.x) == 1)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public virtual void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = .3f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPos = (Application.isPlaying) ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypointPos - Vector3.up * size, globalWaypointPos + Vector3.up * size);
                Gizmos.DrawLine(globalWaypointPos - Vector3.left * size, globalWaypointPos + Vector3.left * size);
            }
        }
    }
}
