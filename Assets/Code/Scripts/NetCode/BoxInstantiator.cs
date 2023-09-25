using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoxInstantiator : NetworkBehaviour
{
    public NetworkObject boxPrefab;
    public Transform[] boxSpawnPoints;

    public void Start()
    {
        if (IsServer)
        {
            SpawnBoxes();
        }
    }

    private void SpawnBoxes()
    {
        foreach (var spawnPoint in boxSpawnPoints)
        {
            var box = Instantiate(boxPrefab);
            box.transform.position = spawnPoint.position;
            box.transform.rotation = spawnPoint.rotation;
            box.Spawn();
        }
    }
}