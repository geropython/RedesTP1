using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class Instantiator : NetworkBehaviour
{
    //CLASE INSTANTIATOR necesaria para el spawn de Network Objects (a mano) en el juego; Tiene una modificacion en donde se recorre un array de player prefabs para instanciar diferentes tipos de player en diferentes spawn points.

    //MODIFICADO PARA QUE SPAWNEE DIFERENTES PREFABS Y EN DIFERENTES SPAWNPOINTS.
    public NetworkObject[] playerPrefabs;

    public Transform[] spawnPoints;

    public void Start()
    {
        ulong id = NetworkManager.Singleton.LocalClientId;
        RequestSpawnPlayerServerRpc(id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong id)
    {
        int playerIndex = NetworkManager.Singleton.ConnectedClients.Keys.ToList().FindIndex(clientId => clientId == id);

        if (playerIndex >= 0 && playerIndex < playerPrefabs.Length && playerIndex < spawnPoints.Length)
        {
            var obj = Instantiate<NetworkObject>(playerPrefabs[playerIndex]);
            obj.transform.position = spawnPoints[playerIndex].position;
            obj.transform.rotation = spawnPoints[playerIndex].rotation;
            obj.SpawnWithOwnership(id);
        }
    }
}