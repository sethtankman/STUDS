﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceTracker : MonoBehaviour
{
    public List<GameObject> ProgressPoints = new List<GameObject>();
    public int Progress = 0;

    public string PLRCol;

    public float ExitTime = 0.5f;
    public float timer;
    public bool Ready = true;


    private void Start()
    {
        CharacterMovementController cmc = GetComponentInParent<CharacterMovementController>();
        PLRCol = cmc.GetColorName();
        timer = ExitTime;
    }

    private void Update()
    {
        if (Ready == false && timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }

        if (timer <= 0.0f)
        {
            ReadySwap();
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("RacePoint") && Ready)
        {
            if (ProgressPoints.Contains(other.gameObject))
            {
                if (Progress > 0)
                {
                    Progress -= 1;
                    ProgressPoints.Remove(other.gameObject);
                }
                else Progress = 0;
            }
            else
            {
                Progress += 1;
                ProgressPoints.Add(other.gameObject);

            }
        }
        if (other.gameObject.name == "RaceReset")
        {
            PLRCol = GetComponentInParent<CharacterMovementController>().GetColorName();
            ProgressPoints.Clear();

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("RacePoint"))
        {
            Ready = false;
            timer = ExitTime;
        }
    }

    void ReadySwap()
    {
        Ready = true;

    }
}
