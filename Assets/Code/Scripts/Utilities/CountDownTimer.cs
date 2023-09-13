using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownTimer : NetworkBehaviour
{
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
    }

    [ClientRpc]
    private void CountdownClientRpc(string countdown)
    {
        countdownText.text = countdown;
    }
}