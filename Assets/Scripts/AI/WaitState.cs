using UnityEngine;

/// <summary>
/// The NPC does nothing in this state.
/// </summary>
public class WaitState : FSMState
{
    private float waitTime;
    private float waitTimeLeft;

    public WaitState(float waitTime)
    {
        stateID = StateID.Waiting;
        this.waitTime = waitTime;
    }

    public override void DoBeforeEntering()
    {
        waitTimeLeft = waitTime;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npc.GetComponent<NPCController>().Health <= 0f)
            npc.GetComponent<NPCController>().SetTransition(Transition.NoHealth);

        if (waitTimeLeft <= 0)
            npc.GetComponent<NPCController>().SetTransition(Transition.DoneWaiting);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        waitTimeLeft -= Time.deltaTime;
    }
}
