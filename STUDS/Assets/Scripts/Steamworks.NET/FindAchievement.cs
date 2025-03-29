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
            if (other.GetComponent<NetworkCharacterMovementController>() && other.GetComponent<NetworkCharacterMovementController>().isLocalPlayer)
            {
                SteamAchievements.UnlockAchievement(achievementName);
            }
            if (other.GetComponent<CharacterMovementController>())
            {
                SteamAchievements.UnlockAchievement(achievementName);
            }
        }
    }
}
