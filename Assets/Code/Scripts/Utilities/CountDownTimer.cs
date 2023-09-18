using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownTimer : NetworkBehaviour
{
    //CONTEO REGRESIVO para antes de inicializar la carrera, espera a que termine la corutina y luego envía el RPC a los clientes para que puedan mover sus players.

    [SerializeField] private TextMeshProUGUI countdownText;
    private Timer timer;

    [System.Obsolete]
    private void Start()
    {
        timer = FindObjectOfType<Timer>();
        if (IsOwner)
        {
            StartCoroutine(StartCountdown());
        }
    }

    [System.Obsolete]
    public IEnumerator StartCountdown()
    {
        countdownText.text = "3";
        yield return new WaitForSeconds(1);
        CountdownClientRpc("2");

        yield return new WaitForSeconds(1);
        CountdownClientRpc("1");

        yield return new WaitForSeconds(1);
        CountdownClientRpc("GO!");

        // Iniciar el cronómetro en todos los clientes
        timer.StartTimerClientRpc();

        yield return new WaitForSeconds(1);
        CountdownClientRpc("");

        // Permitir que los coches se muevan
        AllowMovementClientRpc();
    }

    [ClientRpc]
    private void CountdownClientRpc(string countdown)
    {
        countdownText.text = countdown;
    }

    [ClientRpc]
    [System.Obsolete]
    public void AllowMovementClientRpc() //SOLAMENTE EL SERVER LE DICE QUE COMIENCE
    {
        foreach (var carController in FindObjectsOfType<CarController>())
        {
            carController.canMove = true;
        }
    }
}