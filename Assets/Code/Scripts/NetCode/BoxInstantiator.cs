using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoxInstantiator : NetworkBehaviour
{
    public NetworkObject boostBoxPrefab;
    public NetworkObject explosionBoxPrefab;
    public Transform[] boxSpawnPoints;
    private List<NetworkObject> spawnedBoxes = new List<NetworkObject>();

    public void Start()
    {
        if (IsServer)
        {
            SpawnBoxes();
            StartCoroutine(RespawnBoxes());
        }
    }

    private void SpawnBoxes()
    {
        foreach (var spawnPoint in boxSpawnPoints)
        {
            var box = Instantiate(Random.value < 0.5f ? boostBoxPrefab : explosionBoxPrefab);
            box.transform.position = spawnPoint.position;
            box.transform.rotation = spawnPoint.rotation;
            box.Spawn();
            spawnedBoxes.Add(box);
        }
    }

    private IEnumerator RespawnBoxes()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);

            foreach (var box in spawnedBoxes)
            {
                if (!box.gameObject.activeInHierarchy)
                {
                    box.gameObject.SetActive(true);
                }
            }
        }
    }
}