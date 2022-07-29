
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrollerController : MonoBehaviour
{
    public GameObject player;

    public int StrollerID;
    private string strollerColor;

    
    // Start is called before the first frame update
    void Start()
    {        
        player = GameObject.FindGameObjectWithTag("Player");        
    }

    public void SetID(int id)
    {
        StrollerID = id;
    }

    public void SetColor(string color)
    {
        strollerColor = color;
    }

    public string GetColor()
    {
        return strollerColor;
    }
}
