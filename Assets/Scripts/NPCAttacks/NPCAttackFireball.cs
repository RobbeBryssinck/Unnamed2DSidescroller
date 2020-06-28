public class NPCAttackFireball : NPCAttack
{
    public override void ExecuteAttack()
    {
        Instantiate(weapon, transform.position, transform.rotation, transform);

        var fireball = weapon.GetComponent<Fireball>();
        fireball.direction = gameObject.GetComponent<NPCController>().Direction;
    }
}
