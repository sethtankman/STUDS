using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public static class SteamAchievements
{

    public static void UnlockAchievement(string achievementName)
    {
        if(!SteamManager.Initialized) { return; }

        Debug.Log($"Achieving {achievementName}");

        SteamUserStats.SetAchievement(achievementName);

        SteamUserStats.StoreStats();
    }

}
