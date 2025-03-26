using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetResetGame : NetworkBehaviour
{
    public void Reset()
    {
        if (isServer)
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                player.GetComponent<NetworkCharacterMovementController>().RpcSetToMini(false);
            }
        if (NetGameManager.Instance)
        {
            NetGameManager.Instance.DeletePlayers();
            GameObject ngm = NetGameManager.Instance.gameObject;
            NetGameManager.Instance.RemoveInstance();
            Destroy(ngm);
        } else if (GameObject.Find("NetGameManager"))
        {
            Destroy(GameObject.Find("NetGameManager"));
        }
    }
}
