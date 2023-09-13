using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        //IMPORTANTE PARA EL NON AUTHORITATIVE!
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;
        //ID player
        var playerID = player.OwnerClientId;

        RequestChangeColorServerRpc(playerID);
    }

    [ServerRpc]
    public void RequestChangeColorServerRpc(ulong playerId)
    {
        ClientRpcParams p = new ClientRpcParams();
        List<ulong> list = new List<ulong>();
        foreach (ulong id in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (id != playerId)
            {
                list.Add(id);
            }
        }
        p.Send.TargetClientIds = list;
        ChangeColorClientRpc(playerId, p);
    }

    [ClientRpc]
    public void ChangeColorClientRpc(ulong playerId, ClientRpcParams p = default)
    {
        print(playerId + "  " + NetworkManager.Singleton.LocalClientId);
        if (playerId == NetworkManager.Singleton.LocalClientId) return;
        //ChangeColor;
        print("No es el que collisiono");
    }
}