using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public void Reset()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        if(gameManager) {
            if (gameManager.GetComponent<ManagePlayerHub>())
            {
                gameManager.GetComponent<ManagePlayerHub>().DeletePlayers();
                Destroy(gameManager);
            }
        } else if (NetGameManager.Instance)
        {
            NetGameManager.Instance.DeletePlayers();
            Destroy(GameObject.Find("SteamScripts"));
            Destroy(NetGameManager.Instance.gameObject);
        } else
        {
            Debug.LogWarning("Couldn't find anything in ResetGame");
        }
    }
}
