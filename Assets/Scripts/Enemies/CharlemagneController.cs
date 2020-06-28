public class CharlemagneController : NPCController
{
    protected override void Start()
    {
        base.Start();

        Health = 1000f;

        MakeFSM();
    }

    protected override void MakeFSM()
    {
        ShootFireballState shootFireball = new ShootFireballState(gameObject);
        shootFireball.AddTransition(Transition.DoneShootingFireballs, StateID.MeleeAttack);
        shootFireball.AddTransition(Transition.NoHealth, StateID.Dead);

        MeleeAttackState meleeAttack = new MeleeAttackState();
        meleeAttack.AddTransition(Transition.DoneMeleeAttack, StateID.ShootingFireballs);
        meleeAttack.AddTransition(Transition.NoHealth, StateID.Dead);

        DeadState dead = new DeadState();

        fsm = new FSMSystem();
        fsm.AddState(shootFireball);
        fsm.AddState(meleeAttack);
        fsm.AddState(dead);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }
}
