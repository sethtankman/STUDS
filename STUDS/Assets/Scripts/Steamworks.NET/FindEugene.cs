using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindEugene : MonoBehaviour
{
    public SteamAchievements sa;
    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamAchievements").GetComponent<SteamAchievements>();
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("Found Eugene!");
            sa.UnlockAchievement("SR_EUGENE");
        }
    }
}
