using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour, IRaceLeaderboardUI
{
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI finishPositionText;
    public TextMeshProUGUI positionText;
    public static GameManager Instance { get; private set; }
    public bool raceOver = false;
    public ulong winningPlayerID;
    public GameObject _panelWin;

    public RaceLeaderBoard leaderboard;

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

        leaderboard = new RaceLeaderBoard(this);
    }

    public void IncreaseLap(ulong playerID, CarController carController)
    {
        leaderboard.IncreaseLap(playerID);

        if (leaderboard.GetPlayerLap(playerID) >= 3)
        {
            float finishTime = leaderboard.GetPlayerRaceTime(playerID);
            int finishPosition = leaderboard.GetPlayerPosition(playerID);

            carController.FinishRaceAndDespawn(finishTime, finishPosition);
        }
        else
        {
            UpdatePositionsServerRpc();
            Debug.Log("Winning Player ID:" + winningPlayerID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionsServerRpc()
    {
        int totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        foreach (var player in NetworkManager.Singleton.ConnectedClients)
        {
            ulong playerID = player.Key;
            int position = leaderboard.GetPlayerPosition(playerID);
            UpdatePositionClientRpc(playerID, position, totalPlayers);
        }
    }

    [ClientRpc]
    public void UpdatePositionClientRpc(ulong playerID, int position, int totalPlayers)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            if (positionText != null)
            {
                positionText.text = "Posicion: " + position + "/" + totalPlayers;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishRaceServerRpc(ulong playerID, float time, int position)
    {
        winningPlayerID = playerID;
        raceOver = true;
        Debug.Log(" Completó la carrera");
        ShowWinPanelClientRpc(playerID, time, position);
    }

    public void FinishRace(ulong playerID, float time, int position)
    {
        FinishRaceServerRpc(playerID, time, position);
    }

    [ClientRpc]
    public void ShowWinPanelClientRpc(ulong playerID, float time, int position)
    {
        ShowWinPanel(playerID, time, position);
    }

    public void ShowWinPanel(ulong playerID, float time, int position)
    {
        _panelWin.SetActive(true);

        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            winText.text = "Has finalizado la carrera en " + time.ToString("F2") + " segundos.";
            finishPositionText.text = "Posicion: " + position;
        }
        else
        {
            winText.text = "El jugador " + playerID + " ha finalizado la carrera en " + time.ToString("F2") + " segundos.";
            finishPositionText.text = "Posicion: " + position;
        }
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }

    // Implementación de la interfaz IRaceLeaderboardUI
    public void UpdateLapText(ulong playerID, int lap)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            lapText.text = lap + "/3";
        }
    }

    public void UpdatePositionText(ulong playerID, int position, int totalPlayers)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            positionText.text = "Posicion: " + position + "/" + totalPlayers;
        }
    }
}