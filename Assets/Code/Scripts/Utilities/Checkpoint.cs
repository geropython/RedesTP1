using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    //Evento para suscribirse desde el resetCheckpoint
    public static event Action<Checkpoint> OnCheckpointCleared;

    private bool cleared = false;

    private void OnTriggerEnter(Collider other)
    {
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
