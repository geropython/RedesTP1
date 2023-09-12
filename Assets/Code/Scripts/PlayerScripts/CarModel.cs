using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CarModel : NetworkBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    public float brakeSpeed = 5.0f;
    public float driftForce = 5.0f;
}