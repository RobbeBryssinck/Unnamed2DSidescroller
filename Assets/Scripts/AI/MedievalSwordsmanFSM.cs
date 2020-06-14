using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class MedievalSwordsmanFSM : FSM
{
    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Dead
    }

    // Player objects
    GameObject player;

    // Current state that the NPC is reaching
    public FSMState curState;

    [SerializeField]
    private float moveSpeed;
    private Vector2 velocity;
    private float gravity;
    private Vector2 moveDistance;
    private Collisions collisions;

    [SerializeField]
    protected Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    int waypointIndex;
    int waypointMax;

    Vector3 position;
    Vector3 destination;

    // Whether the NPC is destroyed or not
    private bool isDead;
    private float health;

    protected override void Initialize()
    {
        gravity = -10;

        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;

        curState = FSMState.Patrol;
        moveSpeed = 5.0f;
        isDead = false;
        health = 100f;

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
        waypointIndex = 0;
        waypointMax = globalWaypoints.Length;
        destination = globalWaypoints[waypointIndex];
    }

    protected override void FSMUpdate()
    {
        switch (curState)
        {
            case FSMState.Patrol: UpdatePatrolState(); break;
            case FSMState.Chase: UpdateChaseState(); break;
            case FSMState.Dead: UpdateDeadState(); break;
        }

        if (health <= 0)
            curState = FSMState.Dead;
    }

    protected override void FSMFixedUpdate()
    {
        throw new System.NotImplementedException();
    }

    protected void UpdatePatrolState()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= 5.0f)
            curState = FSMState.Chase;

        if (Vector3.Distance(transform.position, destination) <= 0.2f)
            FindNextPoint();

        CalculateHorizontalMovement();
        CalculateVerticalMovement();
        transform.Translate(position);

        if (collisions.above || collisions.below)
            velocity.y = 0;

        if (Mathf.Sign(position.x) == 1)
            GetComponent<SpriteRenderer>().flipX = true;
        else
            GetComponent<SpriteRenderer>().flipX = false;
    }

    protected void CalculateHorizontalMovement()
    {
        float directionX = Mathf.Sign(destination.x - transform.position.x);
        position.x = directionX * moveSpeed * Time.deltaTime;
    }

    protected void CalculateVerticalMovement()
    {
        velocity.y += gravity * Time.deltaTime;
        moveDistance.y = velocity.y * Time.deltaTime;

        UpdateRaycastOrigins();
        collisions.Reset();

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
                moveDistance.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        position.y = moveDistance.y;
    }

    protected void FindNextPoint()
    {
        waypointIndex++;

        if (waypointIndex == waypointMax)
            waypointIndex = 0;

        destination = globalWaypoints[waypointIndex];
    }

    protected void UpdateChaseState()
    {
        throw new System.NotImplementedException();
    }

    protected void UpdateDeadState()
    {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmos()
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

    protected struct Collisions
    {
        public bool below, above;

        public void Reset()
        {
            below = false;
            above = false;
        }
    }
}
