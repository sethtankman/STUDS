﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItemSpawner : MonoBehaviour
{
    public List<Transform> spawnLocations = new List<Transform>();
    public List<GameObject> items = new List<GameObject>();
    private List<int> isleAccessor = new List<int>();
    private int[] locationMap;

    private void Start()
    {
        SpawnAllItems();
    }

    private void SpawnAllItems()
    {
        locationMap = new int[spawnLocations.Count];
        for(int i = 0; i< spawnLocations.Count; i++)
        {
            int itemNum = i % items.Count;
            locationMap[i] = itemNum;
            ValidatePicks();
        }
        for (int j = 0; j < spawnLocations.Count; j++)
        {
            int random = (int)(Random.value * spawnLocations.Count);
            int temp = locationMap[j];
            locationMap[j] = locationMap[random];
            locationMap[random] = temp;
            GameObject aisle = Instantiate(items[isleAccessor[j]], spawnLocations[j].position, Quaternion.identity);
        }
    }

    void ValidatePicks()
    {
        int picked = Random.Range(0, items.Count);
        while (isleAccessor.Contains(picked))
        {
            picked = Random.Range(0, items.Count);
        }
        isleAccessor.Add(picked);

    }
}
