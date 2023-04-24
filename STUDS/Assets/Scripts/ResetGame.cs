using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public void Reset()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if (gameManager.GetComponent<ManagePlayerHub>())
        {
            gameManager.GetComponent<ManagePlayerHub>().DeletePlayers();
            Destroy(gameManager);
        } else if (gameManager.GetComponent<NetGameManager>())
        {
            gameManager.GetComponent<NetGameManager>().DeletePlayers();
            Destroy(GameObject.Find("SteamScripts"));
            Destroy(gameManager);
        }
    }
}
