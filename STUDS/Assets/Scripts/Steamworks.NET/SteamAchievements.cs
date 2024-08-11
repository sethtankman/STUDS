using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public static class SteamAchievements
{

    public static void UnlockAchievement(string achievementName)
    {
        if(!SteamManager.Initialized) {
            Debug.Log("Steam manager not initialized");
            return; 
        }

        Debug.Log($"Achieving {achievementName}");

        SteamUserStats.SetAchievement(achievementName);

        SteamUserStats.StoreStats();
    }

}
