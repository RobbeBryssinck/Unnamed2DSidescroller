using UnityEditor.Build;
using UnityEngine;

public class PlayerCombatState : FSMState
{
    private Player playerScript;

    public PlayerCombatState(GameObject player)
    {
        stateID = StateID.PlayerCombat;
        playerScript = player.GetComponent<Player>();
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (playerScript.isHit)
            playerScript.SetTransition(Transition.PlayerHit);
    }

    public override void Act(GameObject player, GameObject npc)
    {
    }
}
