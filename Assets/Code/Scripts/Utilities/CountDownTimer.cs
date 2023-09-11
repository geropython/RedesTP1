using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private CarController carController;
    [SerializeField] private Timer timer;

    public IEnumerator StartCountdown()
    {
        carController.enabled = false;
        //FUNCION LLAMADA POR EL CLIENT RPC (FIND OBJECT OF TYPE) --> Manda a todos los controladores que inicialicen.
        //SI SOY DUEÑO, LO INICIALIZO, SINO, NO.
        //UTILIZAR ESTO:
        //NetworkManager.Singleton.IsServer(true)
        countdownText.text = "3";
        yield return new WaitForSeconds(1);

        countdownText.text = "2";
        yield return new WaitForSeconds(1);

        countdownText.text = "1";
        yield return new WaitForSeconds(1);

        countdownText.text = "GO!";
        carController.enabled = true;
        timer.StartTimer();

        yield return new WaitForSeconds(1);

        countdownText.text = "";
    }
}