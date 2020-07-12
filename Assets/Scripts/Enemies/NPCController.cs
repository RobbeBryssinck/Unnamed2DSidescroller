using UnityEngine;

public abstract class NPCController : MonoBehaviour
{
    public Animator animator;

    protected GameObject player;
    protected FSMSystem fsm;

    public float Health { get; set; }
    public int Direction { get; set; } = -1;

    public void SetTransition(Transition t) { fsm.PerformTransition(t); }

    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    protected virtual void Update()
    {
        fsm.CurrentState.Reason(player, gameObject);
        fsm.CurrentState.Act(player, gameObject);
    }

    protected abstract void MakeFSM();

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public abstract void HandleHit(float damage);
}
