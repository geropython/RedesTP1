using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    //ESTE SCRIPT CREO QUE NO NOS SERVIRIA, SOLO ES UNA PRUEBA PERO POEMOS HACER ALGO SIMILAR A LA LINEA DE META PARA VERIFICAR CUANDO UN PLAYER TOCA EL BOX COLLIDER DE LA LLEGADA.
    public void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        var player = other.GetComponent<CarController>();
        if (player == null) return;
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