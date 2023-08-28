using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapCheckpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.IncreaseLap();
            int laps = GameManager.Instance.GetLaps();
            Debug.Log("Has completado " + laps + " vuelta(s). Te quedan " + (3 - laps) + " vuelta(s).");
        }
    }


}
