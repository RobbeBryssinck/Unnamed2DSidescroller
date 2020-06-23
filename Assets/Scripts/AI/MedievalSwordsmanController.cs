using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class MedievalSwordsmanController : MonoBehaviour
{
    GameObject player;
    private FSMSystem fsm;

    [SerializeField]
    protected Vector3[] localWaypoints;
    Vector3[] globalWaypoints;
    int waypointIndex;
    int waypointMax;

    Vector3 position;
    Vector3 destination;

    private bool isDead;
    private float health;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    protected void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

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

        MakeFSM();
    }

    public void FixedUpdate()
    {
        fsm.CurrentState.Reason(player, gameObject);
        fsm.CurrentState.Act(player, gameObject);
    }

    private void MakeFSM()
    {
        PatrolState patrol = new PatrolState(globalWaypoints);
        patrol.AddTransition(Transition.SawPlayer, StateID.Chasing);

        ChaseState chase = new ChaseState();
        chase.AddTransition(Transition.LostPlayer, StateID.Patrolling);

        fsm = new FSMSystem();
        fsm.AddState(patrol);
        fsm.AddState(chase);
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
}
