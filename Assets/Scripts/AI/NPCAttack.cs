using UnityEngine;

public abstract class NPCAttack : MonoBehaviour
{
    [SerializeField]
    protected GameObject weapon;

    public abstract void ExecuteAttack();
}
