using UnityEngine;

/// <summary>
/// The NPC remains in the state for the rest of the game's runtime.
/// It basically does nothing.
/// </summary>
public class DeadState : FSMState
{
    public DeadState()
    {
        stateID = StateID.Dead;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
    }

    public override void Act(GameObject player, GameObject npc)
    {
    }
}
