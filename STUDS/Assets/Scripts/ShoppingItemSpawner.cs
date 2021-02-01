using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItemSpawner : MonoBehaviour
{
    public Transform[] spawnLocations;
    public GameObject[] items;
    private int[] locationMap;

    private void Start()
    {
        SpawnAllItems();
    }

    private void SpawnAllItems()
    {
        Random rand = new Random();
        locationMap = new int[spawnLocations.Length];
        for(int i = 0; i< spawnLocations.Length; i++)
        {
            int itemNum = i % items.Length;
            locationMap[i] = itemNum;
        }
        for (int j = 0; j < spawnLocations.Length; j++)
        {
            int random = (int)(Random.value * spawnLocations.Length);
            int temp = locationMap[j];
            locationMap[j] = locationMap[random];
            locationMap[random] = temp;
            Instantiate(items[locationMap[j]], spawnLocations[j].position, Quaternion.identity);
        }
     

    }
}
