using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //almacenar el tiempo
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    public GameObject _panelWin;
    public static GameManager Instance { get; private set; }
    public ulong winningPlayerID;
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    public bool raceOver = false;
    public TextMeshProUGUI winText;

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
            playerRaceTimes[playerID] = 0f; // Inicializa el tiempo del jugador en 0.
        }

        playerLaps[playerID]++;

        if (playerLaps[playerID] >= 3)
        {
            // El jugador ha ganado la carrera.
            playerRaceTimes[playerID] = Time.time - playerRaceTimes[playerID]; // Calcula el tiempo.
            Win(playerID);
        }
        else
        {
            UpdateLapCountServerRpc(playerID);
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

    //NO SE UTILIZA?¿
    //public int GetLaps(ulong playerID)
    //{
    //    if (playerLaps.ContainsKey(playerID))
    //    {
    //        return playerLaps[playerID];
    //    }
    //    return 0;
    //}

    //para el win panel:
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}