using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject _panelWin;
    public static GameManager Instance { get; private set; }
    public ulong winningPlayerID;
    private int laps = 0;

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

    private void Start()
    {
    }

    public void IncreaseLap(ulong playerID)
    {
        laps++;
        if (laps >= 3)
        {
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
        // Actualiza la cantidad de vueltas restantes ( deberia ser una UI)
        Debug.Log("El jugador " + playerID + " ha completado una vuelta.");
    }

    public void Win(ulong playerID)
    {
        winningPlayerID = playerID;
        NotifyWinServerRpc(playerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyWinServerRpc(ulong playerID)
    {
        NotifyWinClientRpc(playerID);
    }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID)
    {
        // Notifica a todos los jugadores quien ganó (playerID)
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");

        // Si el jugador local es el ganador, muestra el panel de victoria
        if (NetworkManager.Singleton.LocalClientId == playerID)
        {
            _panelWin.SetActive(true);
        }
    }

    public int GetLaps()
    {
        return laps;
    }
}