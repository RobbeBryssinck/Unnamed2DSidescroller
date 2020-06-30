using UnityEngine;

public class MeleeAttackState : FSMState
{
    public MeleeAttackState()
    {
        stateID = StateID.MeleeAttack;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (Vector3.Distance(npc.transform.position, player.transform.position) >= 3.0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.DoneMeleeAttack);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        
    }
}
