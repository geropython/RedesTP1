using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoxSpawner : NetworkBehaviour
{
    public GameObject boxPrefab;
    public Transform[] spawnPoints;
    public float respawnTime = 5.0f;
    private Dictionary<Transform, float> spawnTimers;

    private void Start()
    {
        if (!IsServer) return;

        spawnTimers = new Dictionary<Transform, float>();

        foreach (Transform spawnPoint in spawnPoints)
        {
            SpawnBox(spawnPoint);
            spawnTimers[spawnPoint] = 0;
        }
    }

    private void Update()
    {
        if (!IsServer) return;

        foreach (Transform spawnPoint in spawnPoints)
        {
            if (spawnPoint.childCount == 0)
            {
                spawnTimers[spawnPoint] += Time.deltaTime;

                if (spawnTimers[spawnPoint] >= respawnTime)
                {
                    SpawnBox(spawnPoint);
                    spawnTimers[spawnPoint] = 0;
                }
            }
        }
    }

    private void SpawnBox(Transform spawnPoint)
    {
        GameObject boxInstance = Instantiate(boxPrefab, spawnPoint.position, spawnPoint.rotation);
        boxInstance.GetComponent<NetworkObject>().Spawn();
    }
}
