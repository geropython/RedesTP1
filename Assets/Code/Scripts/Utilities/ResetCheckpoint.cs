using Unity.Netcode;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
{
    //SE ENCARGA DE REESTABLECER EL VEHICULO QUE HAYA CAIDO AL VACIO O HAYA AGARRADO UNA BOX TRAP A SU ULTIMO PUNTO DE CONTROL.

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
        Checkpoint.OnCheckpointCleared -= HandleCheckpointCleared;
    }
}