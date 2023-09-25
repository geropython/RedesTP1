using System.Collections;
using System.Collections.Generic;
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

    //SPEED VARIABLES
    private float currentSpeed;

    private float targetSpeed;

    //RIGIDBODY
    private Rigidbody rb;

    //GM
    public int playerLaps = 0;

    //BOX:
    public Vector3 lastCheckpointPosition;

    public Quaternion lastCheckpointRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //NETWORK BEHAVIOUR:
        if (!IsOwner)
        {
            this.enabled = false;
        }
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
        //LIMITA EL MOVIMIENTO DE LOS AUTOS
        if (GameManager.Instance.raceOver) return;

        // Acelerar
        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = speed;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            targetSpeed = 0;
        }

        // Reversa
        if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = -speed;
        }
        else if (Input.GetKeyUp(KeyCode.S))
        {
            targetSpeed = 0;
        }

        // Freno de mano
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
        // Si canMove es falso, no se mueve el coche
        if (!canMove) return;
        // Mover coche
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);
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
        Debug.Log("CarController: Incrementando la vuelta para el jugador " + myPlayerID + ". Vueltas actuales: " + playerLaps);
        if (playerLaps >= 3)
        {
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
}