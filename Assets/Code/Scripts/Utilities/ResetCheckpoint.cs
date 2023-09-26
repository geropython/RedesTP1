using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
{
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    public GameObject particlePrefab;
    
 
    public AudioSource _risa;

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
        CarController carController = other.GetComponent<CarController>(); // Cambio non authoritative
        if (carController == null || !carController.IsOwner) return;

        if (other.CompareTag("Player"))
        {
            // Guarda la posición actual del jugador antes de restablecerla
            Vector3 playerPositionBeforeReset = other.transform.position;

            // Move the car to the last checkpoint position
            other.transform.position = lastCheckpointPosition;
            other.transform.rotation = lastCheckpointRotation;
           
            //Crash Animacion risa + sonido
            _risa.Play();


            // Calcula la posición donde se instanciará la partícula (sobre el auto)
            Vector3 particlePosition = playerPositionBeforeReset;

            // Poner partícula de respawn en la posición calculada
            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, particlePosition, Quaternion.identity);
            }
        }
      
    }

    private void OnDestroy()
    {
        // Unsubscribe from the OnCheckpointCleared event
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}