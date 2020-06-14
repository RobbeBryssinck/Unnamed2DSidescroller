using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FSM : RaycastController
{
    // Player transform
    protected Transform playerTransform;

    // Next destination position of NPC
    protected Vector3 destPos;

    // List of points for patrolling
    protected GameObject[] pointList;

    protected abstract void Initialize();
    protected abstract void FSMUpdate();
    protected abstract void FSMFixedUpdate();

    protected override void Start()
    {
        base.Start();
        Initialize();
    }
    
    protected void Update()
    {
        FSMUpdate();
    }

    protected void FixedUpdate()
    {
        //FSMFixedUpdate();
    }
}
