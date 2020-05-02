using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField]
    protected float moveSpeed;
    protected abstract float Health { get; set; }

    public abstract void Patrol();

    public abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
