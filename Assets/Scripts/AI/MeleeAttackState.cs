using UnityEngine;

// TODO: Clean up this test stuff
public class MeleeAttackState : FSMState
{
    private float switchTime = 5.0f;
    private float switchTimeLeft = 5.0f;

    public MeleeAttackState()
    {
        stateID = StateID.MeleeAttack;
    }

    public override void DoBeforeEntering()
    {
        switchTimeLeft = switchTime;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (switchTimeLeft <= 0)
            npc.GetComponent<NPCController>().SetTransition(Transition.DoneMeleeAttack);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        switchTimeLeft -= Time.deltaTime;
    }
}
