using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    //PUBLIC FIELDS
    public GameObject _panelWin;

    public TextMeshProUGUI winText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI finishPositionText;
    public TextMeshProUGUI positionText;
    public static GameManager Instance { get; private set; }
    public bool raceOver = false;
    public ulong winningPlayerID;

    //PRIVATE FIELDS:
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();

    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    private Dictionary<ulong, List<float>> playerLapTimes = new Dictionary<ulong, List<float>>();
    private Dictionary<ulong, float> finishedPlayers = new Dictionary<ulong, float>();

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

        if (!playerLapTimes.ContainsKey(playerID))
        {
            playerLapTimes[playerID] = new List<float>();
        }
        playerLapTimes[playerID].Add(Time.time);

        if (playerLaps[playerID] >= 3)
        {
            // Solo añade al jugador a la lista de jugadores terminados si no está ya en la lista
            if (!finishedPlayers.ContainsKey(playerID))
            {
                finishedPlayers.Add(playerID, Time.time - playerRaceTimes[playerID]);
            }

            float finishTime = Time.time - playerRaceTimes[playerID];
            int finishPosition = GetPlayerPosition(playerID);

            // Llama a la función en el carController para finalizar la carrera y despawnear
            carController.FinishRaceAndDespawn(finishTime, finishPosition);
        }
        else
        {
            // Llama a UpdatePositionServerRpc() para actualizar la posición del jugador en la UI
            UpdatePositionServerRpc(playerID);
        }
    }

    public void UpdateLapText(ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            lapText.text = playerLaps[playerID] + "/3";
            // Llama al ServerRpc para actualizar la posición
            //UpdatePositionServerRpc(playerID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRpc(ulong playerID) //REVISAR
    {
        int totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        // Actualiza el texto de la posición
        UpdatePositionClientRpc(playerID, GetPlayerPosition(playerID), totalPlayers);
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

        // Llama al RPC del cliente para mostrar el panel de victoria
        ShowWinPanelClientRpc(playerID, time, position);
        Debug.Log("Jugador " + playerID + " terminó la carrera en la posición " + position);// NO SE MUESTRA.
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
            finishPositionText.text = "Posicion: " + position;
        }
        else
        {
            winText.text = "El jugador " + playerID + " ha finalizado la carrera en " + time.ToString("F2") + " segundos.";
            finishPositionText.text = "Posicion: " + position;
        }
    }

    public int GetPlayerPosition(ulong playerID)
    {
        var sortedPlayers = finishedPlayers
            .OrderBy(x => x.Value)
            .ToList();

        // Encuentra el índice del jugador en la lista de jugadores terminados.
        int playerIndex = sortedPlayers.FindIndex(x => x.Key == playerID);

        // Devuelve la posición del jugador en la carrera, teniendo en cuenta el número total de jugadores que han completado la carrera.
        return playerIndex + 1;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}