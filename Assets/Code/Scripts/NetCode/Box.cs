using Unity.Netcode;
using UnityEngine;

public class Box : NetworkBehaviour
{
    [SerializeField] private GameObject explosionParticlePrefab;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<CarController>();
        if (player == null) return;

        ulong playerID = player.OwnerClientId;

        if (IsServer) // No autoritativo.
        {
            RequestResetCheckpointServerRpc(playerID, player.lastCheckpointPosition, player.lastCheckpointRotation);
        }
    }

    [ServerRpc]
    [System.Obsolete]
    private void RequestResetCheckpointServerRpc(ulong playerID, Vector3 position, Quaternion rotation)
    {
        ApplyResetCheckpointClientRpc(playerID, position, rotation);
        InstantiateParticlesClientRpc();
    }

    [ClientRpc]
    [System.Obsolete]
    private void ApplyResetCheckpointClientRpc(ulong playerID, Vector3 position, Quaternion rotation)
    {
        CarController player = FindPlayerById(playerID);
        if (player != null && player.IsOwner)
        {
            // Restablece al jugador a su checkpoint
            player.transform.position = position;
            player.transform.rotation = rotation;
            InstantiateParticlesClientRpc();
        }
        if (IsServer)
        {
            DespawnServerRpc();
        }
    }

    [ClientRpc]
    [System.Obsolete]
    private void InstantiateParticlesClientRpc()
    {
        Instantiate(explosionParticlePrefab, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    [System.Obsolete]
    private void DespawnServerRpc()
    {
        if (IsServer)
        {
            InstantiateParticlesClientRpc();
            NetworkObject.Despawn();
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