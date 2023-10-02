
using Unity.Netcode;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
{
    private Vector3 lastCheckpointPosition;
    private Quaternion lastCheckpointRotation;
    public GameObject particlePrefab;

    public CrashAnim crashAnim;
    public AudioSource _risa;
    public NetworkObject networkObjectWithAnimator;

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
        CarController carController = other.GetComponent<CarController>();
        if (carController == null || !carController.IsOwner) return;

        if (other.CompareTag("Player"))
        {
            Vector3 playerPositionBeforeReset = other.transform.position;

            other.transform.position = lastCheckpointPosition;
            other.transform.rotation = lastCheckpointRotation;

            if (!carController.IsOwner) return;

            _risa.Play();

            Vector3 particlePosition = playerPositionBeforeReset;

            if (particlePrefab != null)
            {
                Instantiate(particlePrefab, particlePosition, Quaternion.identity);
            }


            // Llama al ServerRpc en la clase CrashAnim
            crashAnim.TriggerLaughAnimationServerRpc();
        }
    }


    private void OnDestroy()
    {
        // Unsubscribe from the OnCheckpointCleared event
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}