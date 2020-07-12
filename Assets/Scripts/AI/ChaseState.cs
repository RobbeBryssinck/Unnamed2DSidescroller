using UnityEngine;

/// <summary>
/// The NPC chases the player until the player is out of a certain range.
/// </summary>
public class ChaseState : FSMState
{
    private float chaseRange;

    private AIMovement aiMovement;

    public ChaseState(float chaseRange, GameObject npc)
    {
        stateID = StateID.Chasing;
        this.chaseRange = chaseRange;
        aiMovement = npc.GetComponent<AIMovement>();
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (Vector3.Distance(player.transform.position, npc.transform.position) >= chaseRange)
            npc.GetComponent<NPCController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        Vector2 velocity = aiMovement.CalculateVelocity();
        aiMovement.Move(velocity * Time.deltaTime);

        if (aiMovement.collisions.touchedPlayerHorizontally)
            npc.GetComponent<NPCController>().animator.SetTrigger("TouchPlayerHorizontally");

        aiMovement.SetDirection(player.transform.position);
    }
}
