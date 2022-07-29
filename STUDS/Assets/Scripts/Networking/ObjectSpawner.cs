using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    public List<Transform> spawnLocations;
    public NetworkManager nm;
    public int[] spawnOrder;

    // Start is called before the first frame update
    void Start()
    {
        nm = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        if (nm && isServer)
        {
            GameObject[] prefabs = nm.spawnPrefabs.ToArray();
            int i = 0;
            foreach (Transform trans in spawnLocations)
            {
                //Debug.Log(i);
                //Debug.Log(spawnOrder[i]);
                //Debug.Log(prefabs[spawnOrder[i]]);
                GameObject throwable = Instantiate(prefabs[spawnOrder[i]], trans.position, trans.rotation);
                NetworkServer.Spawn(throwable);
                i++;
            }
        }
    }
}
