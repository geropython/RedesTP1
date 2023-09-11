using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour  //DEBE SER NETWORK BEHAVIOUR- IS OWNER
{
    //CONTAR DE MANERA LOCAL.EL DUEÑO DEL TIMER (SERVIDOR) DEBERÍA ENVIAR CADA TANTOS SEGUNDOS UN RPC PARA MARCARLO, PARA CORREGIRLO POR SI HAY UN DEFASAJE.
    [SerializeField] private TextMeshProUGUI timerText;

    private float startTime;
    private bool timerStarted = false;

    public void StartTimer()
    {
        startTime = Time.time;
        timerStarted = true;
    }

    private void Update()
    {
        //HACER UN TIMER CADA TRES SEGUNDOS Y CUANDO TERMINA, ENVIAR UN RPC.
        if (timerStarted)
        {
            float t = Time.time - startTime;

            string minutes = ((int)t / 60).ToString();
            string seconds = (t % 60).ToString("f2");

            timerText.text = minutes + ":" + seconds;
        }
    }
}