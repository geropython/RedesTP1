using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoostBox : NetworkBehaviour
{
    [SerializeField] private GameObject speedParticle;
    [SerializeField] private float boostAmount = 2.0f; //cuanto se aumenta la velocidad
    [SerializeField] private float boostDuration = 2.0f; //duración del boost actual.
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
        // Desactiva la caja para todos los jugadores
        gameObject.SetActive(false);

        // Instancia la partícula de explosión
        Instantiate(speedParticle, transform.position, Quaternion.identity);

        // Aumenta la velocidad del coche solo para el jugador que colisionó
        if (playerId == NetworkManager.Singleton.LocalClientId)
        {
            CarController carController = FindObjectOfType<CarController>();
            if (carController != null && carController.OwnerClientId == playerId)
            {
                StartCoroutine(BoostSpeed(carController));
            }
        }
    }

    private IEnumerator BoostSpeed(CarController carController)
    {
        // Guarda la velocidad original del coche
        float originalSpeed = carController.speed;

        // Aumenta la velocidad del coche
        carController.speed *= boostAmount;

        // Espera la duración del aumento de velocidad
        yield return new WaitForSeconds(boostDuration);

        // Restaura la velocidad original del coche
        carController.speed = originalSpeed;
    }
}