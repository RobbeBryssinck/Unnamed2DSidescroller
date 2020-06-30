using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
     PlayerUIHealth hearts;

    private void Start()
    {
        hearts = GameObject.Find("Hearts").GetComponent<PlayerUIHealth>();
    }

    public void TakeDamage(float damage)
    {
        hearts.TakeDamage(damage);
    }

    public void RegenerateHealth()
    {
        hearts.RegenerateHealth();
    }
}
