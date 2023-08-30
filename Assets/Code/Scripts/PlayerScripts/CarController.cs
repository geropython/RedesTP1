using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public CarModel model;
    public CarView view;

    void Update()
    {
        //ADELANTE
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, model.speed * Time.deltaTime);
        }

        //STEERING
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -model.rotationSpeed * Time.deltaTime, 0);
        }

        //REVERSA
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -model.speed * Time.deltaTime);
        }

        //STEERING
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, model.rotationSpeed * Time.deltaTime, 0);
        }

        //FRENO DE MANO
        if (Input.GetKey(KeyCode.Space))
        {
            model.speed -= model.brakeSpeed * Time.deltaTime;
            if (model.speed < 0)
            {
                model.speed = 0;
            }
        }

        view.UpdateCarPosition(transform.position);
        view.UpdateCarRotation(transform.rotation);
    }


}
