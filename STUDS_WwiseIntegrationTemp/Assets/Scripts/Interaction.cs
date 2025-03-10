﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.SceneManagement;

public class Interaction : MonoBehaviour

{
    public PWRBill_Manager GameMaster ; 

    public int PowerCharge = 1;


    public GameObject Object_active;
    public GameObject Object_inactive;

    
    public int TimerDelayAmount = 1;

    public Text BillTotalText;

    protected float CashTimer;

    public bool interactPressed = false;


    private void Awake()
    {
        GameMaster = GameObject.Find("Game Manager").GetComponent<PWRBill_Manager>();
        
    }

    public void Update()
    {
        if (interactPressed)
        {
            
        }

        if (!interactPressed)
        {
            CashTimer += Time.deltaTime;

            if (CashTimer >= TimerDelayAmount)
            {
                CashTimer = 0f;
                GameMaster.AddScore(PowerCharge);
                //print(this.name);

            }
        }
    }

    public void ToggleVisual(bool isMini)
    {
        if (isMini && !Object_active.activeSelf)
        {
            GameMaster.NumItemsOn += 1;
            interactPressed = false;
            Object_active.SetActive(true);
            Object_inactive.SetActive(false);
        }
        else if(!isMini && Object_active.activeSelf)
        {
            GameMaster.NumItemsOn -= 1;
            interactPressed = true;
            Object_active.SetActive(false);
            Object_inactive.SetActive(true);
        }
    }

    public void ToggleVisualGM()
    {
            GameMaster.NumItemsOn -= 1;
            interactPressed = true;
            Object_active.SetActive(false);
            Object_inactive.SetActive(true);
    }
}
