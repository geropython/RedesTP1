using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //CLASE GAME MANAGER que administra distintas funcionalidades del juego, condiciones de victoria, conteo de vueltas, pausa, notificacion con RPC a los clientes para saber quien ganó y en qué tiempos.
   //texto de vueltas a la pantalla
    public TextMeshProUGUI lapText;
   
    //almacenar el tiempo
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    public GameObject _panelWin;
   
    private void Start()
    {
        if (IsOwner && lapText != null)
        {
            // Inicializar el texto de vueltas en la pantalla del jugador local
            lapText.text = "0/3"; // Por ejemplo, comienza con 0 vueltas completadas de 3
        }
    }
    //GAME MANAGER
    public static GameManager Instance { get; private set; }

    public ulong winningPlayerID;
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    public bool raceOver = false;
    public TextMeshProUGUI winText;

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

        if (playerLaps[playerID] >= 3)
        {
            // El jugador ha ganado la carrera.
            playerRaceTimes[playerID] = Time.time - playerRaceTimes[playerID]; // Calcula el tiempo.
            Win(playerID);
        }
       
        // Actualizar el texto de vueltas en todos los clientes utilizando RPC
        UpdateLapTextOnClientsClientRpc(playerLaps[playerID]);
    }

    [ClientRpc]
    private void UpdateLapTextOnClientsClientRpc(int lapCount)
    {
        if (lapText != null)
        {
            lapText.text = lapCount + "/3";
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
        raceOver = true;
        // Detiene el tiempo en el juego en todos los clientes
        StopTimeClientRpc();
        NotifyWinServerRpc(playerID);

        float winningTime = playerRaceTimes[playerID];
        winText.text = "El jugador " + playerID + " ha ganado la carrera en " + winningTime.ToString("F2") + " segundos.";
        _panelWin.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyWinServerRpc(ulong playerID)
    {
        NotifyWinClientRpc(playerID);
    }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID)
    {
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");
    }

    [ClientRpc]
    public void StopTimeClientRpc()
    {
        Time.timeScale = 0;
    }

    //para el win panel:
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}