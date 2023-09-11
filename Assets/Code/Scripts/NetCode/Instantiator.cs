using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Instantiator : NetworkBehaviour
{
    public NetworkObject playerPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            RequestSpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestSpawnPlayerServerRpc(ulong clientId)
    {
        var player = Instantiate(playerPrefab);
        player.SpawnWithOwnership(clientId);
    }
}