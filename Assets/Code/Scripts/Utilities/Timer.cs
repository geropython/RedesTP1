using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Timer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private bool timerStarted = false;
    private float nextRpcTime = 0.0f;
    private float rpcInterval = 3.0f;  // cada cuantos segundos manda los RPC´S
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
            if (IsClient && Time.time >= nextRpcTime)
            {
                nextRpcTime = Time.time + rpcInterval;
                StartTimerClientRpc();
            }
        }
    }

    [ClientRpc]
    public void StartTimerClientRpc()
    {
        timerStarted = true;
        CorrectTimerServerRpc(Time.time - elapsedTime);
    }

    [ServerRpc(RequireOwnership = false)]
    private void CorrectTimerServerRpc(float serverTime)
    {
        CorrectTimerClientRpc(serverTime);
    }

    [ClientRpc]
    private void CorrectTimerClientRpc(float serverTime)
    {
        if (IsServer)
        {
            elapsedTime = Time.time - serverTime;
        }
    }
}