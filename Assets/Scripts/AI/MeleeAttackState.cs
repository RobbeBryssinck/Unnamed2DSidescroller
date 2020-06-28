using UnityEngine;

public class MeleeAttackState : FSMState
{
    public MeleeAttackState()
    {
        stateID = StateID.MeleeAttack;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
    }

    public override void Act(GameObject player, GameObject npc)
    {
    }
}
