using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public void Reset()
    {
        if(ManagePlayerHub.Instance) {
            ManagePlayerHub.Instance.DeletePlayers();
            Destroy(ManagePlayerHub.Instance.gameObject);
        } else if (NetGameManager.Instance)
        {
            NetGameManager.Instance.DeletePlayers();
            //Destroy(GameObject.Find("SteamScripts"));
        } else
        {
            Debug.LogWarning("Couldn't find anything in ResetGame");
        }
    }
}
