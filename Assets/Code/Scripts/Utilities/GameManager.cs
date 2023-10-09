using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //CLASE GAME MANAGER que administra distintas funcionalidades del juego, condiciones de victoria, conteo de vueltas, pausa, notificacion con RPC a los clientes para saber quien ganó y en qué tiempos.

    //CAMBIOS--> Generales, se optimizó un poco el script, pero además la condicion de victoria, junto con la logica de conteo de vueltas por UI para todos los jugadores ynotificacion de victoria.

    //almacenar el tiempo
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();

    public GameObject _panelWin;

    //GAME MANAGER
    public static GameManager Instance { get; private set; }

    public ulong winningPlayerID;
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    public bool raceOver = false;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;

    //SINGLETON PATTERN:
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void IncreaseLap(ulong playerID)
    {
        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time; // Inicializa el tiempo del jugador al comienzo de la carrera.
        }

        playerLaps[playerID]++;
        UpdateLapText(playerID);  // Actualiza el texto de las vueltas.

        if (playerLaps[playerID] >= 3)
        {
            // El jugador ha ganado la carrera.
            playerRaceTimes[playerID] = Time.time - playerRaceTimes[playerID]; // Calcula el tiempo.
            Win(playerID);
        }
    }

    private void UpdateLapText(ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            lapText.text = playerLaps[playerID] + "/3";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateLapCountServerRpc(ulong playerID)
    {
        UpdateLapCountClientRpc(playerID);
    }

    [ClientRpc]
    public void UpdateLapCountClientRpc(ulong playerID)
    {
        Debug.Log("El jugador " + playerID + " ha completado una vuelta.");
    }

    public void Win(ulong playerID)
    {
        winningPlayerID = playerID;
        raceOver = true; //SE DEBE HACER UNA NETWORK VARIABLE. UPDATE --> NEW: SI SE HACE, SE ROMPE LA CONDICION DE VICTORIA.
        float winningTime = playerRaceTimes[playerID];
        winText.text = "El jugador " + playerID + " ha finalizado la carrera en " + winningTime.ToString("F2") + " segundos.";
        _panelWin.SetActive(true);
        NotifyWinAndStopTimeClientRpc(playerID, winningTime);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyWinServerRpc(ulong playerID)
    {
        float winningTime = playerRaceTimes[playerID];
        NotifyWinAndStopTimeClientRpc(playerID, winningTime);
    }

    //SE UNIFICÓ LOS DOS rpc EN UNO SOLO:
    [ClientRpc]
    public void NotifyWinAndStopTimeClientRpc(ulong playerID, float winningTime)
    {
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");
        Time.timeScale = 0;
    }

    //para el win panel:
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}