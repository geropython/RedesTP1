using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager :NetworkBehaviour
{// VARIABLES GM:
    public static GameManager Instance { get; private set; }
    public ulong winningPlayerID;
    public NetworkVariable<bool> raceOver = new NetworkVariable<bool>(default, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public GameObject _panelWin;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;

    // DICTIONARY
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();

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
        if (raceOver.Value) return;

        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time;
        }

        playerLaps[playerID]++;
        UpdateLapText(playerID);

        if (playerLaps[playerID] >= 3)
        {
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

    public void Win(ulong playerID)
    {
        WinPanel();
        WinServerRpc(playerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateLapCountServerRpc(ulong playerID)
    {
        UpdateLapCountClientRpc(playerID);
    }

    public void WinPanel()
    {
        _panelWin.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void WinServerRpc(ulong playerID)
    {
        if (raceOver.Value)
        {
            return;
        }

        raceOver.Value = true;
        winningPlayerID = playerID;
        float winningTime = Time.time - playerRaceTimes[playerID];
        NotifyWinClientRpc(playerID, winningTime);
    }

    [ClientRpc]
    public void UpdateLapCountClientRpc(ulong playerID) { }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID, float winningTime)
    {
        if (!raceOver.Value) return;

        // Comprueba si el jugador local es el ganador
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            _panelWin.SetActive(true);
            Time.timeScale = 0;
            winText.text = "Has ganado la carrera en " + winningTime.ToString("F2") + " segundos.";
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}


