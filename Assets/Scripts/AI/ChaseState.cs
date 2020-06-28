using UnityEngine;

public class ChaseState : FSMState
{
    private AIMovement aiMovement;

    public ChaseState()
    {
        stateID = StateID.Chasing;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (Vector3.Distance(player.transform.position, npc.transform.position) >= 5.0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        aiMovement = npc.GetComponent<AIMovement>();
        aiMovement.Move(player.transform.position);
    }
}
