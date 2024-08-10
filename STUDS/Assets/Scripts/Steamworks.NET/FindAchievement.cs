using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAchievement : MonoBehaviour
{
    public string achievementName;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SteamAchievements.UnlockAchievement(achievementName);
        }
    }
}
