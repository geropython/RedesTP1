using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ResetCheckpoint : NetworkBehaviour
{
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;

    private void Start()
    {
        lastCheckpointPosition = transform.position;
        lastCheckpointRotation = transform.rotation;

        // Subscribe to the Checkpoint event.
        Checkpoint.OnCheckpointCleared += HandleCheckpointCleared;
    }

    private void HandleCheckpointCleared(Checkpoint checkpoint)
    {
        lastCheckpointPosition = checkpoint.transform.position;
        lastCheckpointRotation = checkpoint.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        //NON AUTHORITTATIVE
        if (!IsOwner) return;

        if (other.CompareTag("Player"))
        {
            // Move the car to the last checkpoint position
            other.transform.position = lastCheckpointPosition;
            other.transform.rotation = lastCheckpointRotation;
        }
    }

    private new void OnDestroy()
    {
        // Unsubscribe from the OnCheckpointCleared event
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}