using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCheckpoint : MonoBehaviour
{
    public Checkpoint[] checkpoints;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("LapCheckpoint OnTriggerEnter: Auto " + other.name + " ha entrado en el área del LapCheckpoint");

            // ¿Paso el auto todos los checkpoints?
            bool allCheckpointsCleared = true;
            foreach (Checkpoint checkpoint in checkpoints)
            {
                if (!checkpoint.IsCleared())
                {
                    allCheckpointsCleared = false;
                    break;
                }
            }

            // Si lo hizo, aumenta el contador de vueltas.
            if (allCheckpointsCleared)
            {
                Debug.Log("LapCheckpoint: Todos los Checkpoints están despejados para el auto " + other.name);

                CarController carController = other.GetComponent<CarController>();
                if (carController != null)
                {
                    carController.IncreaseLap();
                }

                // Resetea los checkpoints
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    checkpoint.ResetCheckpoint();
                }
            }
        }
    }
}