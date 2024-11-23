using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetResetGame : MonoBehaviour
{
    public void Reset()
    {
        if (NetGameManager.Instance)
        {
            NetGameManager.Instance.DeletePlayers();
            GameObject ngm = NetGameManager.Instance.gameObject;
            NetGameManager.Instance.RemoveInstance();
            Destroy(ngm);
            //Destroy(GameObject.Find("SteamScripts"));
        } else if (GameObject.Find("NetGameManager"))
        {
            Destroy(GameObject.Find("NetGameManager"));
        }
    }
}
