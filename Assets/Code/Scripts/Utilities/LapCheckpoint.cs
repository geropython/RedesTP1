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
                //NO SIRVE. CADA AUTO DEBE CONTABILIZAR SUS VUELTAS.

                GameManager.Instance.IncreaseLap();
                int laps = GameManager.Instance.GetLaps();
                Debug.Log("Has completado " + laps + " vuelta(s). Te quedan " + (3 - laps) + " vuelta(s).");

                //resetea los checkpoints:
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    checkpoint.ResetCheckpoint();
                }
            }
        }
    }
}