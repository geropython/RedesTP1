using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public GameObject explosionParticlePrefab;

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;
        var playerID = player.OwnerClientId;

        RequestExplosionServerRpc(playerID, player.lastCheckpointPosition, player.lastCheckpointRotation);
    }

    [ServerRpc]
    [System.Obsolete]
    public void RequestExplosionServerRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        TriggerExplosionClientRpc(playerId, position, rotation);
    }

    [ClientRpc]
    [System.Obsolete]
    public void TriggerExplosionClientRpc(ulong playerId, Vector3 position, Quaternion rotation)
    {
        // Desactiva la caja para todos los jugadores
        gameObject.SetActive(false);

        // Instancia la partícula de explosión
        Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);

        // Mueve el auto al último punto de control solo para el jugador que colisionó
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            CarController carController = FindObjectOfType<CarController>();
            if (carController != null && carController.OwnerClientId == playerId)
            {
                carController.transform.position = position;
                carController.transform.rotation = rotation;
            }
        }
    }
}