using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class PatrolState : FSMState
{
    private AIMovement aiMovement;

    private Vector3[] waypoints;
    private int waypointIndex;
    private int waypointMax;
    private Vector3 destination;

    public PatrolState(Vector3[] waypoints)
    {
        this.waypoints = waypoints;
        waypointMax = waypoints.Length;
        waypointIndex = 0;
        destination = waypoints[waypointIndex];
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (Vector3.Distance(player.transform.position, npc.transform.position) <= 5.0f)
            npc.GetComponent<MedievalSwordsmanController>().SetTransition(Transition.SawPlayer);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        if (Vector3.Distance(npc.transform.position, destination) <= 0.2f)
            FindNextPoint();

        aiMovement = npc.GetComponent<AIMovement>();
        aiMovement.Move(destination);
    }

    private void FindNextPoint()
    {
        waypointIndex++;

        if (waypointIndex == waypointMax)
            waypointIndex = 0;

        destination = waypoints[waypointIndex];
    }
}
