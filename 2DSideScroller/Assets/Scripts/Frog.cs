using UnityEngine;

public class Frog : Enemy
{
    protected override float Health { get; set; } = 100;


    private void Start()
    {
        Patrol();
    }

    public override void Patrol()
    {
        print(moveSpeed);
    }

    public override void Attack()
    {
    }
}
