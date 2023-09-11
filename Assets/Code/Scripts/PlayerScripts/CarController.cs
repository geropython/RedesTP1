using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarController : MonoBehaviour  //Tiene que ser NetWorkBehaviour
{
    //MVC
    public CarModel model;

    public CarView view;

    //SPEED VARIABLES
    private float currentSpeed = 0.0f;

    private float targetSpeed = 0.0f;

    //RIGIDBODY
    private Rigidbody rb;

    //NETWORKING
    public ulong OwnerClientId { get; internal set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        ////NETWORK BEHAVIOUR:
        //if (IsOwner --> CAMERA CONTROLLER PARA SEGUIR)
        //{
        //    model = GetComponent<CarModel>();
        //}
        //else
        //{
        //    Destroy(this);   DESTROY NO VA. cuando es netowrk behaviour. poner  ENABLE= FALSE
        //}
    }

    private void Update()
    {
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
        view.UpdateCarPosition(transform.position);
        view.UpdateCarRotation(transform.rotation);
    }
}