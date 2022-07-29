using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

/// <summary>
/// In order to "cope" with the fact that objects that have network identities can't 
/// have other objects with network identities in the child heirarchy, this script spawns 
/// items that would otherwise be children into their correct places.
/// </summary>
public class NetworkChildSpawnCope : NetworkBehaviour
{
    public GameObject[] prefabs;
    public int[] numPrefabs;
    public Transform[] locations;

    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            SpawnPrefabs();
        }
    }

    private void SpawnPrefabs()
    {
        int prefabCount = 0;
        int prefabType = 0;
        foreach(Transform trans in locations)
        {
            if(prefabCount >= numPrefabs[prefabType])
            {
                prefabType++;
                prefabCount = 0;
            }
            //GameObject item = Instantiate(prefabs[prefabType], locations[counter].position + this.transform.position, locations[counter].rotation * this.transform.rotation);
            GameObject item = Instantiate(prefabs[prefabType], trans.position, trans.rotation); 
            item.name = string.Concat(item.name, prefabCount);
            NetworkServer.Spawn(item);
            prefabCount++;
        }
    }
}
