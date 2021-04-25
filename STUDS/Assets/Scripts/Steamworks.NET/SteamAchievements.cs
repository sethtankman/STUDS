using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class SteamAchievements : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UnlockAchievement(string achievementName)
    {
        if(!SteamManager.Initialized) { return; }

        Debug.Log("Achieving Eugene");

        SteamUserStats.SetAchievement(achievementName);

        SteamUserStats.StoreStats();
    }

}
