﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetPlaceTracker : MonoBehaviour
{
    public List<GameObject> ProgressPoints = new List<GameObject>();
    public int Progress = 0;

    public string PLRCol;

    public float ExitTime = 0.5f;
    public float timer;
    public bool Ready = true;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RacePoint") && Ready)
        {
            if (!ProgressPoints.Contains(other.gameObject))
            {
                Progress += 1;
                ProgressPoints.Add(other.gameObject);
            }
        }
        if(other.name == "RaceReset")
        {
            PLRCol = GetComponentInParent<NetworkCharacterMovementController>().GetColorName();
            ProgressPoints.Clear();

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RacePoint"))
        {
            Ready = false;
            timer = ExitTime;
        }
    }

    private void FixedUpdate()
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

    private void Start()
    {
        PLRCol = GetComponentInParent<NetworkCharacterMovementController>().GetColorName();
        timer = ExitTime;
    }

    void ReadySwap()
    {
        Ready = true;

    }
}
