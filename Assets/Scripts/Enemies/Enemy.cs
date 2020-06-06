using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    protected float Health { get; set; }
    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
            Die();
    }

    protected void Die()
    {
        Destroy(gameObject);
    }
}
