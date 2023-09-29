using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    public GameObject explosionParticlePrefab;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    [ServerRpc]
    private void RequestExplosionServerRpc()
    {
        // Este RPC será llamado por el Host (jugador local)
        // Llama al ClientRpc para notificar a todos los clientes
        TriggerExplosionClientRpc();
    }

    [ClientRpc]
    private void TriggerExplosionClientRpc()
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
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;

        Debug.Log("Player " + NetworkManager.Singleton.LocalClientId + " collided with the box.");

        // Restablece al jugador a su checkpoint
        ResetPlayerToCheckpoint(player, player.lastCheckpointPosition, player.lastCheckpointRotation);

        // Llama al ServerRpc para notificar a todos los clientes
        RequestExplosionServerRpc();
    }

    // Método para restablecer al jugador a su checkpoint si es el propietario
    private void ResetPlayerToCheckpoint(CarController player, Vector3 position, Quaternion rotation)
    {
        if (player != null && player.IsOwner)
        {
            // Restablece al jugador a su checkpoint
            player.transform.position = position;
            player.transform.rotation = rotation;
        }
    }
}