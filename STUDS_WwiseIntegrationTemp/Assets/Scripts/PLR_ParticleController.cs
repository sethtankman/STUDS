﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLR_ParticleController : MonoBehaviour
{
    //For the dust cloud
    public ParticleSystem Dustcloud;
    public bool IsRunning = false;

    //For the exlclamation point
    public GameObject Emote;
    public bool inRange = false;
    public Transform PLRCam;

    private void Start()
    {
        Emote.SetActive(false);
        Dustcloud.Stop();
    }

    private void Update()
    {
        Emote.SetActive(inRange);
        Emote.transform.LookAt(PLRCam, Vector2.up);

    }

    //Dustcloud
    public void TurnOnRunning()
    {
        
        if (!IsRunning){
            IsRunning = true;
            Dustcloud.Play();
        }
    }

    public void TurnOffRunning()
    {
        if (IsRunning)
        {
            IsRunning = false;
            Dustcloud.Stop();
        }
    }


    //Exclimation point 
    public void OnTriggerStay(Collider other)
    {
        Interaction InteractiveSC = other.GetComponentInParent<Interaction>();

        if (other.tag == "Electronics" && InteractiveSC.interactPressed == false)
        {
            inRange = true;
            //Emote.SetActive(true);
            
        }
        else
        {
            inRange = false;
        }


    }

    public void OnTriggerExit(Collider other)
    {

        if (other.tag == "Electronics")
        {
            inRange = false;
            //DisableEmote();
        }
    }

    
}