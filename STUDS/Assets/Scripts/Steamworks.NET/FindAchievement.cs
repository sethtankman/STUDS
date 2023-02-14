using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindAchievement : MonoBehaviour
{
    public SteamAchievements sa;
    public string achievementName;

    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Found Eugene!");
            sa.UnlockAchievement(achievementName);
        }
    }
}
