using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;

public class WinPanel : NetworkBehaviour
{
    //    public TextMeshProUGUI winText;
    //    public TextMeshProUGUI positionText;

    //    private int playerPosition;
    //    private float winningTime;

    //    // Método para configurar y mostrar el panel de victoria
    //    public void ShowWinPanel(int position, float time)
    //    {
    //        // Actualizar las variables sincronizadas
    //        playerPosition = position;
    //        winningTime = time;

    //        // Mostrar el panel en todos los clientes
    //        RpcShowWinPanel(position, time);
    //    }

    //    [ClientRpc]
    //    private void RpcShowWinPanel(int position, float time)
    //    {
    //        // Mostrar el panel en todos los clientes
    //        winText.text = "Has finalizado en la posición " + position + " en " + time.ToString("F2") + " segundos.";
    //        positionText.text = "";

    //        // Configurar el texto de posición solo en el jugador local
    //        if (IsOwner)
    //        {
    //            positionText.text = "Posición: " + position;
    //        }

    //        // Activar el panel en todos los clientes
    //        gameObject.SetActive(true);
    //    }


    //}
}
