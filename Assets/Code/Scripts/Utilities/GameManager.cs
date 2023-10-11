using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject _panelWin;
    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI finishPositionText;

    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    private List<ulong> playerProgress = new List<ulong>();
    private Dictionary<ulong, float> finishedPlayers = new Dictionary<ulong, float>();
    public static GameManager Instance { get; private set; }
    public bool raceOver = false;
    public ulong winningPlayerID;

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

    public void IncreaseLap(ulong playerID, CarController carController)
    {
        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time;
        }

        playerLaps[playerID]++;
        UpdateLapText(playerID);

        if (playerLaps[playerID] >= 3)
        {
            // Solo añade al jugador a la lista de jugadores terminados si no está ya en la lista
            if (!finishedPlayers.ContainsKey(playerID))
            {
                finishedPlayers.Add(playerID, Time.time);
            }

            float finishTime = Time.time;
            int finishPosition = GetPlayerPosition(playerID);

            // Llama a la función en el carController para finalizar la carrera y despawnear
            carController.FinishRaceAndDespawn(finishTime, finishPosition);
        }
    }

    public void UpdateLapText(ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            lapText.text = playerLaps[playerID] + "/3";
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishRaceServerRpc(ulong playerID, float time, int position)
    {
        winningPlayerID = playerID;
        raceOver = true;

        // Llama al RPC del cliente para mostrar el panel de victoria
        ShowWinPanelClientRpc(playerID, time, position);
    }

    [ClientRpc]
    public void ShowWinPanelClientRpc(ulong playerID, float time, int position)
    {
        // Muestra el panel a todos los clientes
        ShowWinPanel(playerID, time, position);
    }

    public void ShowWinPanel(ulong playerID, float time, int position)
    {
        _panelWin.SetActive(true);

        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            winText.text = "Has finalizado la carrera en " + time.ToString("F2") + " segundos.";
        }
        else
        {
            winText.text = "El jugador " + playerID + " ha finalizado la carrera en " + time.ToString("F2") + " segundos.";
        }
        finishPositionText.text = "Posicion: " + position;
    }

    public int GetPlayerPosition(ulong playerID)
    {
        if (!finishedPlayers.ContainsKey(playerID))
        {
            finishedPlayers.Add(playerID, playerRaceTimes[playerID]);
        }

        // Crea una lista ordenada de jugadores según las vueltas y el tiempo de carrera
        var sortedPlayers = finishedPlayers.OrderBy(x => x.Value)
                                          .ThenByDescending(x => playerLaps[x.Key])
                                          .ToList();

        int position = sortedPlayers.FindIndex(x => x.Key == playerID) + 1;

        return position;
    }




    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}