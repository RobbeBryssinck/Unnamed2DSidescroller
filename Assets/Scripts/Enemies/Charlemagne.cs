using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charlemagne : Enemy
{
    public GameObject fireball;
    private int direction;

    private void Start()
    {
        Health = 1000;
        direction = -1;
        ShootFireball();
    }

    private void Update()
    {
    }

    private void ShootFireball()
    {
        Instantiate(fireball, transform.position, transform.rotation, transform);

        var fireballcomponent = fireball.GetComponent<Fireball>();
        fireballcomponent.direction = direction;
    }
}
