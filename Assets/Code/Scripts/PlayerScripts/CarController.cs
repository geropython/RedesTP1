using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : NetworkBehaviour
{
    //MVC
    public CarModel model;

    public CarView view;

    //SPEED VARIABLES
    private float currentSpeed;

    private float targetSpeed;

    //RIGIDBODY
    private Rigidbody rb;

    //GM
    public int playerLaps = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        //NETWORK BEHAVIOUR:
        if (IsOwner) //-- > CAMERA CONTROLLER PARA SEGUIR)
        {
            model = GetComponent<CarModel>();
        }
        else
        {
            this.enabled = false;
        }
    }

    private void Update()
    {
        //Si YO soy el dueño del Player
        if (!IsOwner) return;

        HandleInput();
        UpdateSpeed();
        MoveCar();
        UpdateView();
    }

    private void HandleInput()
    {
        // Acelerar
        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = model.speed;
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            targetSpeed = 0;
        }

        // Reversa
        if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = -model.speed;
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
            transform.Rotate(0, -model.rotationSpeed * Time.deltaTime, 0);
            rb.AddForce(-transform.right * model.driftForce);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, model.rotationSpeed * Time.deltaTime, 0);
            rb.AddForce(transform.right * model.driftForce);
        }
    }

    private void UpdateSpeed()
    {
        // Actualizar velocidad actual
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, model.brakeSpeed * Time.deltaTime);
    }

    private void MoveCar()
    {
        // Mover coche
        transform.Translate(0, 0, currentSpeed * Time.deltaTime);
    }

    private void UpdateView()
    {
        //Actualiza la vista del auto
        view.UpdateCarPosition(transform.position);
        view.UpdateCarRotation(transform.rotation);
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
}