using UnityEngine;

/// <summary>
/// The NPC patrols certain waypoints.
/// </summary>
public class PatrolState : FSMState
{
    private AIMovement aiMovement;

    private Vector3[] waypoints;
    private int waypointIndex;
    private int waypointMax;
    private Vector3 destination;

    public PatrolState(Vector3[] waypoints, GameObject npc)
    {
        this.waypoints = waypoints;
        waypointMax = waypoints.Length;
        waypointIndex = 0;
        destination = waypoints[waypointIndex];
        aiMovement = npc.GetComponent<AIMovement>();
        stateID = StateID.Patrolling;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (Vector3.Distance(player.transform.position, npc.transform.position) <= 5.0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.SawPlayer);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        if (Vector3.Distance(npc.transform.position, destination) <= 0.2f)
            FindNextPoint();

        Vector2 velocity = aiMovement.CalculateVelocity();
        aiMovement.Move(velocity * Time.deltaTime);
        aiMovement.SetDirection(destination);
    }

    private void FindNextPoint()
    {
        waypointIndex++;

        if (waypointIndex == waypointMax)
            waypointIndex = 0;

        destination = waypoints[waypointIndex];
    }
}
