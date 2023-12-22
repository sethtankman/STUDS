using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateDodgeballSpawner : MonoBehaviour
{
    public GameObject DodgeballSpawner;
    public GameObject SpawnTransform;

    public void InstantiateDodgeballSpawn()
    {
        Instantiate(DodgeballSpawner, SpawnTransform.transform.position, Quaternion.identity);
    }
}
