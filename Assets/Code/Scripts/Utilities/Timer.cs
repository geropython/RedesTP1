using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Timer : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText;

    private NetworkVariable<float> startTime;
    private bool timerStarted = false;
    private float nextRpcTime = 0.0f;
    private float rpcInterval = 3.0f;  // cada cuantos segundos manda los RPC�S

    private void Awake()
    {
        startTime = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }

    public void StartTimer()
    {
        if (IsServer)
        {
            startTime.Value = Time.time;
            timerStarted = true;
            StartTimerClientRpc();
        }
    }

    private void Update()
    {
        if (timerStarted)
        {
            float t = Time.time - startTime.Value;

            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");

            timerText.text = minutes + ":" + seconds;

            // Si es el cliente y pas� dicho intervalo de tiempo, env�a un RPC
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
        CorrectTimerServerRpc(Time.time - startTime.Value);
    }

    //SI ESTO NO EST� EN FALSE,TIRA NULL EN LOS CLIENTES
    [ServerRpc(RequireOwnership = false)]
    private void CorrectTimerServerRpc(float serverTime)
    {
        startTime.Value = Time.time - serverTime;
        CorrectTimerClientRpc(serverTime);
    }

    [ClientRpc]
    private void CorrectTimerClientRpc(float serverTime)
    {
        // Corrige el timer local con el timer del server.
        //startTime.Value = Time.time - serverTime;
    }
}