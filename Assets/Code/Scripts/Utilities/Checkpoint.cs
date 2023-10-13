using System;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Event to subscribe from the ResetCheckpoint
    public static event Action<Checkpoint> OnCheckpointCleared;

    private bool cleared = false;

    private void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();//COMPROBACION NUEVA- NON AUTHORITATIVE.
        if (carController == null || !carController.IsOwner) return;

        if (other.CompareTag("Player") && !cleared)
        {
            cleared = true;
            OnCheckpointCleared?.Invoke(this);
            // Imprime un mensaje en la consola cada vez que un jugador pasa por un punto de control
        }
    }

    public bool IsCleared()
    {
        return cleared;
    }

    public void ResetCheckpoint()
    {
        cleared = false;
    }
}