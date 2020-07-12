public class FrogGreenController : NPCController
{
    private float timeBetweenJumps = 4.0f;

    protected override void Start()
    {
        base.Start();

        Health = 100f;

        MakeFSM();
    }

    public override void HandleHit(float damage)
    {
        TakeDamage(damage);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void MakeFSM()
    {
        FrogMoveState frogMove = new FrogMoveState(gameObject, timeBetweenJumps);
        frogMove.AddTransition(Transition.NoHealth, StateID.Dead);

        DeadState dead = new DeadState();

        fsm = new FSMSystem();
        fsm.AddState(frogMove);
        fsm.AddState(dead);
    }
}
