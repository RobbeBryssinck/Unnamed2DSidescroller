using UnityEngine;

public class ShootFireballState : FSMState
{
    private NPCAttack npcAttack;

    private bool isDoneShooting;
    private int shotsFired;
    private int maxShots;
    private float timeBetweenShots;
    private float timeBetweenShotsLeft;

    public ShootFireballState(GameObject npc, int maxShots, float timeBetweenShots)
    {
        stateID = StateID.ShootingFireballs;
        npcAttack = npc.GetComponent<NPCAttack>();
        this.maxShots = maxShots;
        this.timeBetweenShots = timeBetweenShots;
    }

    public override void DoBeforeEntering()
    {
        isDoneShooting = false;
        shotsFired = 0;
        timeBetweenShotsLeft = timeBetweenShots;
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

        timeBetweenShotsLeft -= Time.deltaTime;

        if (timeBetweenShotsLeft <= 0f)
        {
            npcAttack.ExecuteAttack();

            shotsFired++;
            if (shotsFired >= maxShots)
                isDoneShooting = true;

            timeBetweenShotsLeft = timeBetweenShots;
        }
    }
}
