using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    // Variables para almacenar el estado del juego
    public static GameManager Instance { get; private set; }

    public ulong winningPlayerID;
    public bool raceOver = false;

    // Variables para la interfaz de usuario
    public GameObject _panelWin;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;

    // Variables para almacenar el progreso de los jugadores
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();

    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();

    // Inicializa la instancia singleton
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

    // Llamado cuando un jugador completa una vuelta
    public void IncreaseLap(ulong playerID)
    {
        if (raceOver) return;

        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time;
        }

        playerLaps[playerID]++;
        lapText.text = playerLaps[playerID] + "/3";

        if (playerLaps[playerID] >= 3)
        {
            Win(playerID);
        }
    }

    // Llamado cuando un jugador gana la carrera
    public void Win(ulong playerID)
    {
        //Debera hacer Server RPC y que ese server llame al CLient RPC.
        raceOver = true;  //NETWORK VARIABLE --> SETEARLO CUANDO SE LLAME AL  SERVER RPC . YA NO DEBERA SER UN BOOLEANO
        winningPlayerID = playerID;
        float winningTime = Time.time - playerRaceTimes[playerID];
        StopTimeClientRpc();  //HACER UNO SOLO.(QUE REPRODUZCA TODO)
        NotifyWinClientRpc(playerID, winningTime);
    }

    // RPCs para notificar a los clientes sobre el progreso de la carrera
    [ServerRpc(RequireOwnership = false)]
    public void UpdateLapCountServerRpc(ulong playerID)
    {
        UpdateLapCountClientRpc(playerID);
    }

    [ClientRpc]
    public void UpdateLapCountClientRpc(ulong playerID)
    {
        Debug.Log("El jugador " + playerID + " completó una vuelta.");
    }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID, float winningTime)
    {
        Debug.Log("El jugador " + playerID + " ganó la carrera.");
        winText.text = "El jugador " + playerID + " ganó la carrera en " + winningTime.ToString("F2") + " segundos.";
        if (!raceOver) return;

        _panelWin.SetActive(true);
    }

    //UNIFICAR EL sTOP TIME CLIENT RPC Y EL NOTIFY WIN CLIENT RPC EN UNA SOLA FUNCION, NO HACEN FALTA LOS DOS.
    [ClientRpc]
    public void StopTimeClientRpc()
    {
        Time.timeScale = 0;
    }

    // Método para volver al menú principal
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}