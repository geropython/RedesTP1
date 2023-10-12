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
    public TextMeshProUGUI positionText; // Nuevo texto para mostrar la posici�n actual

    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();

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
        UpdatePositionServerRpc(playerID);

        if (playerLaps[playerID] >= 3)
        {
            // Solo a�ade al jugador a la lista de jugadores terminados si no est� ya en la lista
            if (!finishedPlayers.ContainsKey(playerID))
            {
                finishedPlayers.Add(playerID, Time.time);
            }

            float finishTime = Time.time;
            int finishPosition = GetPlayerPosition(playerID);

            // Llama a la funci�n en el carController para finalizar la carrera y despawnear
            carController.FinishRaceAndDespawn(finishTime, finishPosition);
        }
    }

    public void UpdateLapText(ulong playerID)
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            lapText.text = playerLaps[playerID] + "/3";
            // Llama al ServerRpc para actualizar la posici�n
            UpdatePositionServerRpc(playerID);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePositionServerRpc(ulong playerID) //REVISAR
    {
        // Obtiene el n�mero total de jugadores
        int totalPlayers = NetworkManager.Singleton.ConnectedClients.Count;
        // Actualiza el texto de la posici�n
        UpdatePositionClientRpc(playerID, GetPlayerPosition(playerID), totalPlayers);
    }

    [ClientRpc]
    public void UpdatePositionClientRpc(ulong playerID, int position, int totalPlayers)  //REVISAR
    {
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            positionText.text = "Posicion: " + position + "/" + totalPlayers;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void FinishRaceServerRpc(ulong playerID, float time, int position)
    {
        winningPlayerID = playerID;
        raceOver = true;

        // Llama al RPC del cliente para mostrar el panel de victoria
        ShowWinPanelClientRpc(playerID, time, position);
        Debug.Log("Jugador " + playerID + " termin� la carrera en la posici�n " + position); //ESTE DEBUGEO NO SALT� NUNCA EN CONSOLA; PUEDE QUE AC� HAYA UN PROBLEMA?
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
        if (!finishedPlayers.ContainsKey(playerID))
        {
            finishedPlayers.Add(playerID, playerRaceTimes[playerID]);
        }

        var sortedPlayers = finishedPlayers
            .OrderByDescending(x => playerLaps[x.Key])
            .ThenBy(x => x.Value)
            .ToList();

        // Obtiene el n�mero de jugadores que han completado la carrera.
        int totalPlayers = sortedPlayers.Count;

        // Encuentra el �ndice del jugador en la lista de jugadores terminados.
        int playerIndex = sortedPlayers.FindIndex(x => x.Key == playerID);

        // Devuelve la posici�n del jugador en la carrera, teniendo en cuenta el n�mero total de jugadores que han completado la carrera.
        return playerIndex + 1;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
    }
}