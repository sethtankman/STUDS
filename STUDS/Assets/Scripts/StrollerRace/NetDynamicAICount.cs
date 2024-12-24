using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// Deactivates AI to ensure there are always 4 players.
/// </summary>
public class NetDynamicAICount : NetworkBehaviour
{
    public List<GameObject> AiOBJ = new List<GameObject>();
    public int PlayerCount;

    /// <summary>
    /// Should only be called by server.
    /// </summary>
    public void FillWithAI()
    {
        PlayerCount = 0;
        foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerCount += 1;
        }

        if (PlayerCount > 4)
        {
            int numAI2Remove = PlayerCount - 4;
            for (int i = 0; i < numAI2Remove; i++)
            {
                AiOBJ[i].SetActive(false); // Keep this here so the server will be able to immediately have an accurate count of players.
                AiOBJ[i].GetComponentInChildren<NetPlayerAI>().RpcSetActive(false);
            }
        }
    }
}
