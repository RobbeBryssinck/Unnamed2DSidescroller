using UnityEngine;

public abstract class NPCController : MonoBehaviour
{
    protected GameObject player;
    protected FSMSystem fsm;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    protected abstract void Start();
    
    protected virtual void FixedUpdate()
    {
        fsm.CurrentState.Reason(player, gameObject);
        fsm.CurrentState.Act(player, gameObject);
    }

    protected abstract void MakeFSM();
}
