using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public GameObject explosionParticle; // Referencia a la partícula de explosión

    public void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;
        var playerID = player.OwnerClientId;

        RequestExplosionServerRpc(playerID, player.lastCheckpointPosition, player.lastCheckpointRotation);
    }

    [ServerRpc]
    public void RequestExplosionServerRpc(ulong playerId, Vector3 position, Quaternion rotation)
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
        TriggerExplosionClientRpc(playerId, position, rotation, p);
    }

    [ClientRpc]
    public void TriggerExplosionClientRpc(ulong playerId, Vector3 position, Quaternion rotation, ClientRpcParams p = default)
    {
        if (playerId == NetworkManager.Singleton.LocalClientId) return;

        // Desactiva la caja
        gameObject.SetActive(false);

        // Activa la partícula de explosión
        explosionParticle.SetActive(true);

        // Mueve el auto al último punto de control
        CarController carController = GetComponent<CarController>();
        if (carController != null)
        {
            carController.transform.position = position;
            carController.transform.rotation = rotation;
        }
    }
}