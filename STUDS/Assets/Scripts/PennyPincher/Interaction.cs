using System;
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
    public PWRBill_Manager GameMaster;

    public int PowerCharge = 1;


    public GameObject Object_active;
    public GameObject Object_inactive;
    public VolumeTrigger trigger;


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
        Debug.Log("ToggleVisual Called.");
        if (isMini && !Object_active.activeSelf)
        {
            GameMaster.NumItemsOn += 1;
            interactPressed = false;
            trigger.isSwitchActive = false;
            Object_active.SetActive(true);
            Object_inactive.SetActive(false);
            PennyPincherAI[] allAI = FindObjectsOfType(typeof(PennyPincherAI)) as PennyPincherAI[];
            foreach(PennyPincherAI AI in allAI)
            {
                AI.CheckUpdateTarget(gameObject, false);
            }
        }
        else if (!isMini && Object_active.activeSelf)
        {
            GameMaster.NumItemsOn -= 1;
            interactPressed = true;
            trigger.isSwitchActive = true;
            Object_active.SetActive(false);
            Object_inactive.SetActive(true);
            PennyPincherAI[] allAI = FindObjectsOfType(typeof(PennyPincherAI)) as PennyPincherAI[];
            foreach (PennyPincherAI AI in allAI)
            {
                AI.CheckUpdateTarget(gameObject, true);
            }
        }
        else if (isMini)
        {
            Debug.Log("Child trying to turn on object that is already on");
        } else
        {
            Debug.Log("Parent trying to turn off object that is already off");
        }
    }

    public void ToggleVisualGM()
    {
        GameMaster.NumItemsOn -= 1;
        interactPressed = true;
        Debug.Log("Toggle: " + gameObject.name);
        trigger.isSwitchActive = true;
        Object_active.SetActive(false);
        Object_inactive.SetActive(true);
        PennyPincherAI[] allAI = FindObjectsOfType(typeof(PennyPincherAI)) as PennyPincherAI[];
        foreach (PennyPincherAI AI in allAI)
        {
            AI.CheckUpdateTarget(gameObject, false);
        }
    }
}
