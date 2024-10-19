using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetInstantiateDBSpawner : NetworkBehaviour
{
    public GameObject DodgeballSpawner;
    public GameObject SpawnTransform;
    public GameObject SpawnEffects;

    public void InstantiateDodgeballSpawn()
    {
        if (isServer)
        {
            GameObject effects = Instantiate(SpawnEffects, SpawnTransform.transform.position, Quaternion.identity);
            GameObject spawner = Instantiate(DodgeballSpawner, SpawnTransform.transform.position, Quaternion.identity);
            NetworkServer.Spawn(effects);
            NetworkServer.Spawn(spawner);
        }
    }
}
