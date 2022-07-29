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
            bool isServer = gameManager.GetComponent<NetGameManager>().DeletePlayers();
            Destroy(gameManager);
            StudsNetworkManager netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
            if (!netManager)
                Debug.LogError("net manager not found.");
            else
            {
                Debug.Log("Found the network manager");
            }
            netManager.StopClient();
            if(isServer)
                netManager.StopServer();
        }
    }
}
