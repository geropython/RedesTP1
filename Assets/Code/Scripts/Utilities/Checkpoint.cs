using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
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