using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarView : MonoBehaviour
{
    public void UpdateCarPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void UpdateCarRotation(Quaternion rotation)
    {
        transform.rotation = rotation;
    }
}