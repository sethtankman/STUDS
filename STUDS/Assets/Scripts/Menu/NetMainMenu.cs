using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetMainMenu : MonoBehaviour
{
    // Array of public GameObjects to check
    public GameObject[] gameObjectsToCheck;

    void Start()
    {
        // Enable any GameObjects in the array that are currently disabled
        foreach (GameObject obj in gameObjectsToCheck)
        {
            if (obj != null && !obj.activeSelf) // Check if the GameObject is not null and is inactive
            {
                obj.SetActive(true); // Enable the GameObject
            }
        }
    }

    public void QuitGame()
    {
        SteamLobby.singleton.CleanupLobby();
        Application.Quit();
    }
}
