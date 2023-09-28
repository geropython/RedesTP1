using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Instantiator : NetworkBehaviour
{
    public NetworkObject[] playerPrefabs;
    public Transform[] spawnPoints;

    private bool[] playersSpawned;

    private void Start()
    {
        InitializePlayerSpawnStatus();
        ulong id = NetworkManager.Singleton.LocalClientId;
        RequestSpawnPlayerServerRpc(id);
    }

    private void InitializePlayerSpawnStatus()
    {
        playersSpawned = new bool[playerPrefabs.Length];
        for (int i = 0; i < playersSpawned.Length; i++)
        {
            playersSpawned[i] = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong id)
    {
        int playerIndex = NetworkManager.Singleton.ConnectedClients.Keys.ToList().FindIndex(clientId => clientId == id);

        if (playerIndex >= 0 && playerIndex < playerPrefabs.Length && playerIndex < spawnPoints.Length)
        {
            if (!playersSpawned[playerIndex])
            {
                var obj = Instantiate<NetworkObject>(playerPrefabs[playerIndex]);
                obj.transform.position = spawnPoints[playerIndex].position;
                obj.transform.rotation = spawnPoints[playerIndex].rotation;
                obj.SpawnWithOwnership(id);
                playersSpawned[playerIndex] = true;
            }
            else
            {
                // El jugador ya se ha instanciado, tal vez mostrar un mensaje de error.
                Debug.LogWarning("Player already spawned.");
            }
        }
        else
        {
            // Manejo de errores: Índices fuera de rango.
            Debug.LogError("Invalid player index or spawn point.");
        }
    }
}