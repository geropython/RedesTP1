using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class Checkpoint : NetworkBehaviour
{
    // Event to subscribe from the ResetCheckpoint
    public static event Action<Checkpoint> OnCheckpointCleared;

    private bool cleared = false;

    private void OnTriggerEnter(Collider other)
    {
        //NON AUTHORITATIVE!!
        if (!IsOwner) return;

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