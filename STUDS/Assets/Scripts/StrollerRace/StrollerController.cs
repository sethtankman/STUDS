
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrollerController : MonoBehaviour
{
    public GameObject player;

    [SerializeField] private int StrollerID;
    public string strollerColor;

    public Material strollerColor1;
    public Material strollerColor2;
    public Material strollerColor3;
    public Material strollerColor4;
    public Material strollerColor5;


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

    public int GetID()
    {
        return StrollerID;
    }

    public void DetermineColor(string colorName)
    {
        if (colorName.Equals("blue"))
        {
            GetComponent<MeshRenderer>().material = strollerColor1;
        }
        else if (colorName.Equals("green"))
        {
            GetComponent<MeshRenderer>().material = strollerColor2;
        }
        else if (colorName.Equals("red"))
        {
            GetComponent<MeshRenderer>().material = strollerColor3;
        }
        else if (colorName.Equals("yellow"))
        {
            GetComponent<MeshRenderer>().material = strollerColor4;
        }
        else if (colorName.Equals("purple"))
        {
            GetComponent<MeshRenderer>().material = strollerColor5;
        }
        SetColor(colorName);
    }
}
