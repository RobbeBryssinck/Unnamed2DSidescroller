using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected abstract float Health { get; set; }

    [SerializeField]
    protected Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    [SerializeField]
    protected float moveSpeed;
    int fromWaypointIndex;
    float percentBetweenWaypoints;

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
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentBetweenWaypoints += Time.deltaTime * moveSpeed;

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], percentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (fromWaypointIndex >= globalWaypoints.Length-1)
            {
                fromWaypointIndex = 0;
                System.Array.Reverse(globalWaypoints);
            }
        }

        newPos -= transform.position;
        transform.Translate(newPos);
    }

    public abstract void Attack();

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
