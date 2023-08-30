using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetCheckpoint : MonoBehaviour
{
   [SerializeField] private Transform resetPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = resetPosition.position;
            other.transform.rotation = resetPosition.rotation;
        }
    }
}
