using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public GameObject _panelWin;
    public static GameManager Instance { get; private set; }
    public ulong winningPlayerID;
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    public bool raceOver = false;

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
        }

        playerLaps[playerID]++;

        if (playerLaps[playerID] >= 3)
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
        Debug.Log("El jugador " + playerID + " ha completado una vuelta.");
    }

    [ClientRpc]
    public void ShowWinPanelClientRpc()
    {
        _panelWin.SetActive(true);
    }

    public void Win(ulong playerID)
    {
        Debug.Log("GameManager: Llamando a NotifyWinServerRpc para el jugador " + playerID);

        Debug.Log("GameManager: Jugador " + playerID + " ha alcanzado 3 vueltas. Activando condición de victoria.");
        winningPlayerID = playerID;
        NotifyWinServerRpc(playerID);

        // En lugar de mostrar el panel de victoria, imprime un mensaje en la consola
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");
        
        // Cuando un jugador gana, establece raceOver en true
        _panelWin.SetActive(true);
        raceOver = true;
        NotifyWinClientRpc(playerID);
    }

    [ServerRpc(RequireOwnership = false)]
    public void NotifyWinServerRpc(ulong playerID)
    {
        Debug.Log("GameManager: Notificando victoria del jugador " + playerID + " en el servidor.");
        Debug.Log("GameManager: NotifyWinServerRpc llamado para el jugador " + playerID);
        NotifyWinClientRpc(playerID);
    }

    [ClientRpc]
    public void NotifyWinClientRpc(ulong playerID)
    {
        Debug.Log("GameManager: NotifyWinClientRpc llamado para el jugador " + playerID);
        Debug.Log("GameManager: Notificando victoria del jugador " + playerID + " en los clientes.");
        // Notifica a todos los jugadores quien ganó (playerID)
        Debug.Log("El jugador " + playerID + " ha ganado la carrera.");

        Debug.Log("Comienza corutina");
        // Inicia la corrutina localmente en cada cliente
        StartCoroutine(LoadWinSceneAfterDelay(2));
    }

    private IEnumerator LoadWinSceneAfterDelay(float delay)
    {
        // Espera el número especificado de segundos
        yield return new WaitForSeconds(delay);

        // Carga la escena de victoria
        SceneManager.LoadScene("WinGame");
    }

    //UTILIZAR CON UNA UI PARA REGISTRAR VUELTAS DE CADA JUGADOR QUIZAS?¿
    public int GetLaps(ulong playerID)
    {
        if (playerLaps.ContainsKey(playerID))
        {
            return playerLaps[playerID];
        }
        return 0;
    }
    public void ReturnMenu()
    {
        SceneManager.LoadScene(0);
    }
}