using UnityEngine;

[RequireComponent(typeof(AIMovement))]
public class MedievalSwordsmanController : NPCController
{
    [SerializeField]
    protected Vector3[] localWaypoints;
    Vector3[] globalWaypoints;

    protected override void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Health = 100f;

        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }

        MakeFSM();
    }

    protected override void MakeFSM()
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
