using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Timer : NetworkBehaviour 
{
    [SerializeField] private TextMeshProUGUI timerText;

    private NetworkVariable<float> startTime = new NetworkVariable<float>(0);
    private bool timerStarted = false;
    private float nextRpcTime = 0.0f;
    private float rpcInterval = 3.0f;  // cada cuantos segundos manda los RPC´S

    public void StartTimer()
    {
        startTime.Value = Time.time;
        timerStarted = true;
    }

    private void Update()
    {
        if (timerStarted)
        {
            float t = Time.time - startTime.Value;

            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");

            timerText.text = minutes + ":" + seconds;

            // Si es el servidor pasó dicho  intervalo de tiempo, envía un RPC
            if (IsServer && Time.time >= nextRpcTime)
            {
                nextRpcTime = Time.time + rpcInterval;
                CorrectTimerServerRpc(t);
            }
        }
    }

    [ServerRpc]
    private void CorrectTimerServerRpc(float serverTime)
    {
        CorrectTimerClientRpc(serverTime);
    }

    [ClientRpc]
    private void CorrectTimerClientRpc(float serverTime)
    {
        // Corrige el timer local con el timer del server.
        startTime.Value = Time.time - serverTime;
    }
}