using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIHealth : MonoBehaviour
{
    List<Transform> hearts;
    int heartIndex;
    int maxHearts;
    // TODO: somehow make hearts generate based on amount of health put in in the player class
    float health;

    private void Start()
    {
        heartIndex = 0;
        health = GameObject.Find("player").GetComponent<Player>().Health;

        hearts = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            hearts.Add(transform.GetChild(i));
        }

        maxHearts = hearts.Count;
    }

    public void TakeDamage(float damage)
    {
        hearts[heartIndex].GetComponent<Image>().enabled = false;
        heartIndex += 1;

        //TODO: put in a real death handler here
        if (heartIndex == maxHearts)
        {
            RegenerateHealth();
        }
    }

    public void RegenerateHealth()
    {
        foreach(var heart in hearts)
        {
            heart.GetComponent<Image>().enabled = true;
        }

        heartIndex = 0;
    }
}
