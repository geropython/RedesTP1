using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour
{
    //TIMER que manda una señal de sincronizacion cada X segundos para que actualice tanto en el host como en los clientes.
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerStarted = false;
    private float nextRpcTime = 0.0f;
    private float rpcInterval = 3.0f;  // cada cuantos segundos manda los RPC´S --> LAG entre host y clients ?¿
    private float elapsedTime = 0.0f;

    public void StartTimer()
    {
        if (IsServer)
        {
            timerStarted = true;
            StartTimerClientRpc();
        }
    }

    private void Update()
    {
        if (timerStarted)
        {
            elapsedTime += Time.deltaTime;

            string minutes = ((int)elapsedTime / 60).ToString();
            string seconds = (elapsedTime % 60).ToString("f2");

            timerText.text = minutes + ":" + seconds;

            // Si es el cliente y pasó dicho intervalo de tiempo, envía un RPC
            if (IsServer && Time.time >= nextRpcTime)
            {
                nextRpcTime = Time.time + rpcInterval;
                CorrectTimerClientRpc(elapsedTime); // Cambia elapsedTime a Time.time
            }
        }
    }

    [ClientRpc]
    public void StartTimerClientRpc()
    {
        timerStarted = true;
    }

    [ClientRpc]
    private void CorrectTimerClientRpc(float serverTime)
    {
        elapsedTime = serverTime; // Usa serverTime directamente
    }
}