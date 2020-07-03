using UnityEngine;

public class CharlemagneController : NPCController
{
    private AIMovement aiMovement;
    [SerializeField]
    private int maxShots;

    protected override void Start()
    {
        base.Start();

        Health = 1000f;
        aiMovement = GetComponent<AIMovement>();

        MakeFSM();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        aiMovement.SimulateGravity();
    }

    protected override void MakeFSM()
    {
        ShootFireballState shootFireball = new ShootFireballState(gameObject, 3, 0.3f);
        shootFireball.AddTransition(Transition.DoneShootingFireballs, StateID.Waiting);
        shootFireball.AddTransition(Transition.NoHealth, StateID.Dead);

        WaitState wait = new WaitState(5.0f);
        wait.AddTransition(Transition.DoneWaiting, StateID.ShootingFireballs);
        wait.AddTransition(Transition.NoHealth, StateID.Dead);

        DeadState dead = new DeadState();

        fsm = new FSMSystem();
        fsm.AddState(shootFireball);
        fsm.AddState(wait);
        fsm.AddState(dead);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    public override void HandleHit()
    {
        TakeDamage(100f);
    }
}
