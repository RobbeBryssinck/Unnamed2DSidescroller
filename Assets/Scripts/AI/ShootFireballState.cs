using UnityEngine;

public class ShootFireballState : FSMState
{
    private NPCAttack npcAttack;

    private bool isDoneShooting;

    public ShootFireballState(GameObject npc)
    {
        stateID = StateID.ShootingFireballs;
        npcAttack = npc.GetComponent<NPCAttack>();
    }

    public override void DoBeforeEntering()
    {
        isDoneShooting = false;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (isDoneShooting)
            npc.GetComponent<NPCController>().SetTransition(Transition.DoneShootingFireballs);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        if (isDoneShooting)
            return;

        npcAttack.ExecuteAttack();
        isDoneShooting = true;
    }
}
