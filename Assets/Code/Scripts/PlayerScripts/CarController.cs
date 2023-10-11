using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    // CarModel fields
    public float speed = 10.0f;

    public float rotationSpeed = 100.0f;
    public float brakeSpeed = 5.0f;
    public float driftForce = 5.0f;

    public bool canMove = false;
    private bool isBoosting = false;

    //SPEED VARIABLES
    private float currentSpeed;

    private float originalSpeed;
    public float targetSpeed;  //DEJAR

    //RIGIDBODY
    private Rigidbody rb;

    //GM
    public int playerLaps = 0;

    private GameManager gameManager;

    public float finishTime;
    public NetworkObject networkObject;
    public int finishPosition;


    //BOX:
    public Vector3 lastCheckpointPosition;

    public Quaternion lastCheckpointRotation;

    private void Start()
    {
        if (!IsOwner)
        {
            this.enabled = false;
        }
        rb = GetComponent<Rigidbody>();
        networkObject = GetComponent<NetworkObject>();
        originalSpeed = speed;
        gameManager = GameManager.Instance;
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (GameManager.Instance.raceOver) return;

        HandleInput();
        UpdateSpeed();
        MoveCar();
        UpdateView();
    }

    private void HandleInput()
    {
        // Verifica si la carrera ha terminado antes de procesar la entrada
        if (GameManager.Instance.raceOver)
        {
            return;
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = isBoosting ? originalSpeed * 2.0f : originalSpeed;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            targetSpeed = 0;
        }

        // Reverse
        if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = -speed;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            targetSpeed = 0;
        }

        // Handbrake
        if (Input.GetKey(KeyCode.Space))
        {
            targetSpeed = 0;
        }

        // Steering LEFT/RIGHT
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotationSpeed * Time.deltaTime, 0);
            rb.AddForce(-transform.right * driftForce);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
            rb.AddForce(transform.right * driftForce);
        }
    }

    private void UpdateSpeed()
    {
        // Actualizar velocidad actual
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, brakeSpeed * Time.deltaTime);
    }

   

    private void MoveCar()
    {
        if (!canMove) return;

        Vector3 movement = transform.forward * currentSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    private void UpdateView()
    {
        //Actualiza la vista del auto
        UpdateCarPosition(transform.position);
        UpdateCarRotation(transform.rotation);
    }

    // PARA DESACTIVAR AL AUTO AL GANAR
    [ServerRpc]
    public void DespawnCarServerRpc()
    {
        networkObject.Despawn();
    }

    //HACER RPC Al servidor para decirle en que tiempo finalicé y así comprobar en que posicion salí.Ademas un Server RPC para eliminar al auto (despawn en true).

    public void FinishRaceAndDespawn(float finishTime, int finishPosition)
    {
        // Envía el mensaje RPC para finalizar la carrera y despawnear el auto
        GameManager.Instance.FinishRaceServerRpc(networkObject.OwnerClientId, finishTime, finishPosition);

        // Muestra el panel con la posición y el tiempo
        GameManager.Instance.ShowWinPanel(networkObject.OwnerClientId, finishTime, finishPosition);

        // Inicia la corutina para despawnear el auto después de un retraso
        StartCoroutine(DespawnCar());
    }

    //CORUTINA PARA DELAY BREVE ANTES DE QUE EL AUTO DESPAWNEE Y ASI PODER VER EL PANEL.
    public IEnumerator DespawnCar()
    {
        yield return new WaitForSeconds(3);

        DespawnCarServerRpc();
    }

    // CarView methods
    public void UpdateCarPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateCarRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }

    public void ApplyBoost(float boostAmount, float boostDuration)
    {
        if (isBoosting) return;

        StartCoroutine(BoostCoroutine(boostAmount, boostDuration));
    }

    private IEnumerator BoostCoroutine(float boostAmount, float boostDuration)
    {
        isBoosting = true;

        float originalSpeed = speed;
        speed += boostAmount;

        yield return new WaitForSeconds(boostDuration);

        speed = originalSpeed;
        isBoosting = false;
    }
}