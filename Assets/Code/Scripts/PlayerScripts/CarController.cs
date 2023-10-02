using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    // AHORA SE JUNTO EL VIEW Y EL MODEL EN UN SOLO SCRIPT, EL "CONTROLLER" YA QUE LOS OTROS DOS NO TENIAN UNA LOGICA MUY EXTENSA.

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
        originalSpeed = speed;
    }

    private void Update()
    {
        //NON AUTHORITATIVE- PRIMERO COMPRUEBA SI ES EL DUEÑO Y DESPUES EL RESTO.
        if (!IsOwner) return;

        if (GameManager.Instance.raceOver) return;

        HandleInput();
        UpdateSpeed();
        MoveCar();
        UpdateView();
    }

    private void HandleInput()
    {
        // Limit car movement
        if (GameManager.Instance.raceOver) return;

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

    //PARA CONTABILIZAR LAS VUELTAS EN LAP CHECKPOINT
    public void IncreaseLap()
    {
        playerLaps++;
        ulong myPlayerID = GetComponent<NetworkObject>().OwnerClientId;
        if (playerLaps >= 3)
        {
            //ESTO ANDA BIEN
            GameManager.Instance.Win(myPlayerID);
        }
        else
        {
            GameManager.Instance.IncreaseLap(myPlayerID);
        }
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