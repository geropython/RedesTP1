using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RaceLeaderBoard : NetworkBehaviour
{
    private Dictionary<ulong, int> playerLaps = new Dictionary<ulong, int>();
    private Dictionary<ulong, float> playerRaceTimes = new Dictionary<ulong, float>();
    private Dictionary<ulong, List<float>> playerLapTimes = new Dictionary<ulong, List<float>>();
    private Dictionary<ulong, float> finishedPlayers = new Dictionary<ulong, float>();

    private IRaceLeaderboardUI ui;

    public RaceLeaderBoard(IRaceLeaderboardUI ui)
    {
        this.ui = ui;
    }

    public void IncreaseLap(ulong playerID)
    {
        if (!playerLaps.ContainsKey(playerID))
        {
            playerLaps[playerID] = 0;
            playerRaceTimes[playerID] = Time.time;
        }

        playerLaps[playerID]++;

        if (!playerLapTimes.ContainsKey(playerID))
        {
            playerLapTimes[playerID] = new List<float>();
        }
        playerLapTimes[playerID].Add(Time.time);

        if (playerLaps[playerID] >= 3)
        {
            if (!finishedPlayers.ContainsKey(playerID))
            {
                finishedPlayers.Add(playerID, Time.time - playerRaceTimes[playerID]);
            }
        }

        ui.UpdateLapText(playerID, GetPlayerLap(playerID));
        ui.UpdatePositionText(playerID, GetPlayerPosition(playerID), GetTotalPlayers());
    }

    public int GetPlayerPosition(ulong playerID)
    {
        var sortedPlayers = playerLaps
            .Where(x => !finishedPlayers.ContainsKey(x.Key))
            .OrderBy(x => GetTotalRaceTime(x.Key)) // Ordenar por tiempo total de la carrera
            .ToList();

        int playerPosition = sortedPlayers.FindIndex(x => x.Key == playerID) + 1;
        return playerPosition;
    }

    public int GetPlayerLap(ulong playerID)
    {
        return playerLaps.ContainsKey(playerID) ? playerLaps[playerID] : 0;
    }

    public float GetPlayerRaceTime(ulong playerID)
    {
        return playerRaceTimes.ContainsKey(playerID) ? playerRaceTimes[playerID] : 0;
    }

    //public List<float> GetPlayerLapTimes(ulong playerID)
    //{
    //    return playerLapTimes.ContainsKey(playerID) ? playerLapTimes[playerID] : new List<float>();
    //}

    public float GetTotalRaceTime(ulong playerID)
    {
        return playerRaceTimes.ContainsKey(playerID) ? playerRaceTimes[playerID] : 0;
    }

    public int GetTotalPlayers()
    {
        return playerLaps.Count;
    }
}