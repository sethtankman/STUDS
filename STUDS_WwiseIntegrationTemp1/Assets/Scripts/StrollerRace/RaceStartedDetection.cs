﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceStartedDetection : MonoBehaviour
{

    public GameObject AI1;

    public GameObject AI2;
    
    public GameObject AI3;

    public GameObject gameManager;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerEnter(Collider obj)
    {
        if (obj.tag.Equals("Player"))
        {
            AI1.GetComponent<PlayerAI>().StartAI();
            AI2.GetComponent<PlayerAI>().StartAI();
            AI3.GetComponent<PlayerAI>().StartAI();
            gameManager.GetComponent<LevelStart>().DisableJoin();
        }

    }
}
