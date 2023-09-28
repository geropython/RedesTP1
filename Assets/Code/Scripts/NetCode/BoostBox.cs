using System.Collections;
using System.Collections.Generic;
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
        var player = other.GetComponent<CarController>();
        if (player == null) return;

        ulong playerID = player.OwnerClientId;
        OnTriggerEnterServerRpc(playerID);
    }

    [ServerRpc]
    [System.Obsolete]
    private void OnTriggerEnterServerRpc(ulong playerID)
    {
        OnTriggerEnterClientRpc(playerID);
    }

    [ClientRpc]
    [System.Obsolete]
    private void OnTriggerEnterClientRpc(ulong playerID)
    {
        CarController player = FindPlayerById(playerID);
        if (player != null)
        {
            if (player.IsOwner)
            {
                player.ApplyBoost(boostAmount, boostDuration);
            }
            Instantiate(speedParticle, transform.position, Quaternion.identity);
            gameObject.SetActive(false);
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