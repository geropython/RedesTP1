using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoostBox : NetworkBehaviour
{
    [SerializeField] private GameObject speedParticle;
    [SerializeField] private float boostAmount = 2.0f;
    [SerializeField] private float boostDuration = 2.0f;
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0, 100, 0);

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }

    [System.Obsolete]
    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;

        var player = other.GetComponent<CarController>();
        if (player == null) return;
        var playerID = player.OwnerClientId;

        RequestBoostServerRpc(playerID);
    }

    [ServerRpc]
    [System.Obsolete]
    public void RequestBoostServerRpc(ulong playerId)
    {
        TriggerBoostClientRpc(playerId);
    }

    [ClientRpc]
    [System.Obsolete]
    public void TriggerBoostClientRpc(ulong playerId)
    {
        // Instantiate the speed particle
        Instantiate(speedParticle, transform.position, Quaternion.identity);

        // Increase the car's speed only for the player who collided
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            CarController carController = FindObjectOfType<CarController>();
            if (carController != null && carController.OwnerClientId == playerId)
            {
                StartCoroutine(BoostSpeed(carController));
            }
        }

        StartCoroutine(DeactivateBox());
    }

    private IEnumerator DeactivateBox()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }

    private IEnumerator BoostSpeed(CarController carController)
    {
        // Save the car's original speed
        float originalSpeed = carController.speed;

        // Increase the car's speed
        carController.speed *= boostAmount;
        Debug.Log("Boost applied, new speed: " + carController.speed);

        // Wait for the boost duration
        yield return new WaitForSeconds(boostDuration);

        // Restore the car's original speed
        carController.speed = originalSpeed;
        Debug.Log("Boost ended, original speed restored: " + carController.speed);
    }
}