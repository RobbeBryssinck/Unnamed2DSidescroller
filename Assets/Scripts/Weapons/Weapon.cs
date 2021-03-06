﻿using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public LayerMask collisionMask;

    protected RaycastController rcController;

    [SerializeField]
    protected float damage;

    public virtual void HandleHit() { }
}
