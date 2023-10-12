using Unity.Netcode;
using UnityEngine;

public class BoostBox : NetworkBehaviour
{
    [SerializeField] private GameObject speedParticle;
    [SerializeField] private float boostAmount = 10.0f;
    [SerializeField] private float boostDuration = 2.0f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (IsServer) // Check if this is the server
        {
            var player = other.GetComponent<CarController>();
            if (player == null) return;

            ulong playerID = player.OwnerClientId;

            ApplyBoostClientRpc(playerID);
        }
    }

    [ClientRpc]
    [System.Obsolete]
    private void ApplyBoostClientRpc(ulong playerID)
    {
        CarController player = FindPlayerById(playerID);
        if (player != null && player.IsOwner)
        {
            player.ApplyBoost(boostAmount, boostDuration);
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
        Instantiate(speedParticle, transform.position, Quaternion.identity);
        gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    [System.Obsolete]
    private void DespawnServerRpc()
    {
        Instantiate(speedParticle, transform.position, Quaternion.identity);
        NetworkObject.Despawn();
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