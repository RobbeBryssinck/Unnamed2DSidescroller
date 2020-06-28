using UnityEngine;

public abstract class NPCController : MonoBehaviour
{
    protected GameObject player;
    protected FSMSystem fsm;

    public float Health { get; set; }

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    protected virtual void FixedUpdate()
    {
        fsm.CurrentState.Reason(player, gameObject);
        fsm.CurrentState.Act(player, gameObject);
    }

    protected abstract void MakeFSM();
}
