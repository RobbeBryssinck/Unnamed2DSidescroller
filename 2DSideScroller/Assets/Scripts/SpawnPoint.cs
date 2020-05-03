using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [HideInInspector]
    public Vector3 spawnPoint;

    void OnDrawGizmos()
    {
        spawnPoint = gameObject.transform.position;

        Gizmos.color = Color.green;
        float size = .3f;

        Vector3 spawnPointPos = spawnPoint + transform.position;
        Gizmos.DrawLine(spawnPoint - Vector3.up * size, spawnPoint + Vector3.up * size);
        Gizmos.DrawLine(spawnPoint - Vector3.left * size, spawnPoint + Vector3.left * size);
    }
}
