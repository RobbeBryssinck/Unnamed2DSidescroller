using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedievalSoldierFSM : MonoBehaviour
{
    public enum FSMState
    {
        None,
        Patrol,
        Chase,
        Dead
    }

    // Current state that the NPC is reaching
    public FSMState curState;

    // Speed of the soldier
    private float curSpeed;

    // Whether the NPC is destroyed or not
    private bool isDead;
    private int health;
}
