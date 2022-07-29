using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetRandomPicker : NetworkBehaviour
{
    public int MaxDespawned;
    public List<GameObject> ToBePicked = new List<GameObject>();
    private List<int> picks = new List<int>();

    // Start is called before the first frame update
    public void RandomDeactivate()
    {
        if (isServer) {
            for (int i = 0; i < MaxDespawned; i++)
            {
                ValidatePicks();
            }

            for (int j = 0; j < MaxDespawned; j++)
            {
                Debug.Log("Sending despawn to client");
                RpcDeactivateObject(picks[j]);
            }

        }
    }

    //Picks and checks which objects to spawn
    void ValidatePicks()
    {
        int picked = Random.Range(0, ToBePicked.Count);
        while (picks.Contains(picked))
        {
            picked = Random.Range(0, ToBePicked.Count);
        }
        picks.Add(picked);

    }

    [ClientRpc]
    private void RpcDeactivateObject(int index)
    {
        Debug.Log($"Rpc Deactivate called on {index}");
        ToBePicked[index].SetActive(false);
    }
}
