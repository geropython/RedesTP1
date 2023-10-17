using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRaceLeaderboardUI
{
    void UpdateLapText(ulong playerID, int lap);
    void UpdatePositionText(ulong playerID, int position, int totalPlayers);
    void ShowWinPanel(ulong playerID, float time, int position);


}
