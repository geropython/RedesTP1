using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    //BOX QUE FUNCIONA COMO TRAMPA, YA QUE REESTABLECE LA VEHICULO AL ULTIMO CHECKPOINT(RESETCHECKPOINT) Y ADEMAS INTERFIERE LA VISTA CON UNA GRAN EXPLOSION.

    public GameObject explosionParticlePrefab;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void RequestExplosionServerRpc(ulong playerID)
    {
        // Este RPC será llamado por el Host
        // Llama al ClientRpc para notificar a todos los clientes
        TriggerExplosionClientRpc(playerID);
    }

    [ClientRpc]
    private void TriggerExplosionClientRpc(ulong playerID)
    {
        // Este RPC será llamado en todos los clientes
        // Desactiva la caja para todos los jugadores
        gameObject.SetActive(false);

        // Instancia la partícula de explosión
        Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CarController>();
        if (player == null) return;

        ulong playerID = player.OwnerClientId;

        if (IsOwner) //no autoritativo.
        {
            ResetPlayerToCheckpointClientRpc(playerID, player.lastCheckpointPosition, player.lastCheckpointRotation);
            RequestExplosionServerRpc(playerID);
        }
    }

    [ClientRpc]
    [System.Obsolete]
    private void ResetPlayerToCheckpointClientRpc(ulong playerID, Vector3 position, Quaternion rotation)
    {
        CarController player = FindPlayerById(playerID);
        if (player != null)
        {
            // Restablece al jugador a su checkpoint
            player.transform.position = position;
            player.transform.rotation = rotation;
        }
    }

    [System.Obsolete]
    private CarController FindPlayerById(ulong playerID)
    {
        CarController[] players = FindObjectsOfType<CarController>();
        foreach (CarController player in players)
        {
            if (player.OwnerClientId == playerID)
            {
                return player;
            }
        }
        return null;
    }
}