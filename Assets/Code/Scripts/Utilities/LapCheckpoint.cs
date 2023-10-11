using UnityEngine;

public class LapCheckpoint : MonoBehaviour
{
    public Checkpoint[] checkpoints;

    private void OnTriggerEnter(Collider other)
    {
        CarController carController = other.GetComponent<CarController>();
        if (carController == null || !carController.IsOwner) return;

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
                GameManager.Instance.IncreaseLap(carController.networkObject.OwnerClientId, carController);

                // Resetea los checkpoints
                foreach (Checkpoint checkpoint in checkpoints)
                {
                    checkpoint.ResetCheckpoint();
                }
            }
        }
    }
}