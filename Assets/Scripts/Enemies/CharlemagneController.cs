using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CharlemagneController : NPCController
{
    public GameObject fireball;

    protected override void Start()
    {
        base.Start();

        Health = 1000f;

        MakeFSM();
    }

    protected override void MakeFSM()
    {
        throw new NotImplementedException();
    }

    public override void TakeDamage(float damage)
    {
        throw new NotImplementedException();
    }

    // TODO: Move to FSM
    private void ShootFireball()
    {
        Instantiate(fireball, transform.position, transform.rotation, transform);

        /*
        var fireballcomponent = fireball.GetComponent<Fireball>();
        fireballcomponent.direction = direction;
        */
    }
}
