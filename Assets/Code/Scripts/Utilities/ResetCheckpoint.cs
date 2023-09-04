using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
{
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;

    private void Start()
    {
        
        lastCheckpointPosition = transform.position;
        lastCheckpointRotation = transform.rotation;

        // Se suscribe al evento de Checkpoint cs.
        Checkpoint.OnCheckpointCleared += HandleCheckpointCleared;
    }

    private void HandleCheckpointCleared(Checkpoint checkpoint)
    {
        
        lastCheckpointPosition = checkpoint.transform.position;
        lastCheckpointRotation = checkpoint.transform.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Mover el auto a la �ltima posici�n del punto de control
            other.transform.position = lastCheckpointPosition;
            other.transform.rotation = lastCheckpointRotation;
        }
    }

    private void OnDestroy()
    {
        // Anular la suscripci�n al evento OnCheckpointCleared
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}
