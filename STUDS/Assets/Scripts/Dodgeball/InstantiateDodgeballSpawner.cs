using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateDodgeballSpawner : MonoBehaviour
{
    public GameObject DodgeballSpawner;
    public GameObject SpawnTransform;
    public GameObject SpawnEffects;

    public void InstantiateDodgeballSpawn()
    {
        Instantiate(SpawnEffects, SpawnTransform.transform.position, Quaternion.identity);
        Instantiate(DodgeballSpawner, SpawnTransform.transform.position, Quaternion.identity);
        //Destroy(SpawnEffects);
    }
}
