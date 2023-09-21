using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
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

    public void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();  //CAMBIO NON AUTHORITATIVE.
        if (carController == null || !carController.IsOwner) return;

        if (other.CompareTag("Player"))
        {
            // Move the car to the last checkpoint position
            other.transform.position = lastCheckpointPosition;
            other.transform.rotation = lastCheckpointRotation;
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnCheckpointCleared event
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}