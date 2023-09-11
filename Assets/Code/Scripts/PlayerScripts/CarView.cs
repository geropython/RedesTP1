using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarView : MonoBehaviour  //Tiene que ser NetWorkBehaviour
{
    //aa:
    public void UpdateCarPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateCarRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}