using UnityEngine;

/// <summary>
/// Makes the player invulnerable to any damage
/// </summary>
public class InvulnerableState : FSMState
{
    private float invulnerableTime;
    private float invulnerableTimeLeft;

    private Player playerScript;

    public InvulnerableState(float invulnerableTime, GameObject player)
    {
        stateID = StateID.Invulnerable;
        this.invulnerableTime = invulnerableTime;
        playerScript = player.GetComponent<Player>();
    }

    public override void DoBeforeEntering()
    {
        invulnerableTimeLeft = invulnerableTime;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (invulnerableTimeLeft <= 0)
            playerScript.SetTransition(Transition.DoneInvulnerable);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        invulnerableTimeLeft -= Time.deltaTime;
    }

    public override void DoBeforeLeaving()
    {
        playerScript.isHit = false;
    }
}
