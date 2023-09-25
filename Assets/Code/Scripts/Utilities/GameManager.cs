using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //CLASE GAME MANAGER que administra distintas funcionalidades del juego, condiciones de victoria, conteo de vueltas, pausa, notificacion con RPC a los clientes para saber quien ganó y en qué tiempos.

    //almacenar el tiempo
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();

    public Dictionary<ulong, TextMeshProUGUI> playerLapTexts = new Dictionary<ulong, TextMeshProUGUI>();
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
        if (raceOver) return;

        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time; // Inicializa el tiempo del jugador al comienzo de la carrera.
        }

        playerLaps[playerID]++;

        // Actualiza el texto de la UI
        lapText.text = playerLaps[playerID] + "/3";

        if (playerLaps[playerID] >= 3)
        {
            // El jugador ha ganado la carrera.
            playerRaceTimes[playerID] = Time.time - playerRaceTimes[playerID]; // Calcula el tiempo.
            Win(playerID);
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
        //PRUEBA
        _panelWin.SetActive(true);
        raceOver = true;
        winningPlayerID = playerID;
        // Detiene el tiempo en el juego en todos los clientes
        float winningTime = playerRaceTimes[playerID];
        StopTimeClientRpc();
        NotifyWinClientRpc(playerID, winningTime);
    }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID, float winningTime)
    {
        if (!raceOver) return;
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");
        winText.text = "El jugador " + playerID + " ha ganado la carrera en " + winningTime.ToString("F2") + " segundos.";
        _panelWin.SetActive(true); // Activa el panel de victoria para todos los clientes.
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