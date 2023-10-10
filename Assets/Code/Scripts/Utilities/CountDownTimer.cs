using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownTimer : NetworkBehaviour
{
    //CAMBIOS en el RPC: Ahora se llama al StartTimer dentro del mismo y se corrigió para que siga un Non authoritative.

    [SerializeField] private TextMeshProUGUI countdownText;
    private Timer timer;

    [System.Obsolete]
    private void Start()
    {
        timer = FindObjectOfType<Timer>();
        StartCoroutine(StartCountdown());
    }

    [System.Obsolete]
    public IEnumerator StartCountdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "GO!";

        yield return new WaitForSeconds(1);

        countdownText.text = "";

        // Allow cars to move
        AllowMovementClientRpc();
        //ME PREGUNTO SI SOY EL SERVER, Y SI LO SOY HAGO EL STARTIMER
        if (IsServer)
            timer.StartTimer();
    }

    [ClientRpc]
    [System.Obsolete]
    public void AllowMovementClientRpc()
    {
        foreach (var carController in FindObjectsOfType<CarController>())
        {
            // Start the timer on all clients
            // timer.StartTimer();

            //SOY EL DUEÑO - IS OWNER?
            if (carController.IsOwner)
            {
                carController.canMove = true;
                break;
            }
        }
    }
}