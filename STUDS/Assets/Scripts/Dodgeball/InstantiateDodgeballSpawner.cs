using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiateDodgeballSpawner : MonoBehaviour
{
    public GameObject DodgeballSpawner;
    public GameObject SpawnTransform;
    public GameObject SpawnEffects;
    [SerializeField] private bool AIIgnore;

    public void InstantiateDodgeballSpawn()
    {
        Instantiate(SpawnEffects, SpawnTransform.transform.position, Quaternion.identity);
        GameObject spawner = Instantiate(DodgeballSpawner, SpawnTransform.transform.position, Quaternion.identity);
        spawner.GetComponent<SpawnDrops>().setAIIgnore(AIIgnore);
        //Destroy(SpawnEffects);
    }
}
