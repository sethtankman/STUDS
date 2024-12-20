﻿using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class ManholeGeiser : MonoBehaviour
{

    public bool up = false;
    public float speed;
    public float delay;
    public float length;
    private float timer;

    public GameObject waterCol;
    public ParticleSystem[] water;
    public ParticleSystem BuildUpWater;
    public GameObject cover;
    public VirtualAudioSource virtualAudioSource;
    public AK.Wwise.Event manholeSound;


    public float size;
    public float WarmupTime = 1;

    private Transform waterBaseT;
    public Vector3 coverBaseT;
    public Vector3 coverTargetOffset;
    public Vector3 angle;

    public Animator MHCover;

    // Start is called before the first frame update
    void Awake()
    {
        angle = this.transform.eulerAngles;
        //waterBaseT = water.transform;
        coverBaseT = cover.transform.position;
        timer = delay;
        BuildUpWater.Stop();

    }

    // Update is called once per frame
    void Update()
    {
        

        delay -= Time.deltaTime;

        if (delay > 0 && delay < WarmupTime && up == false)
        {
            MHCover.SetBool("Rumble", true);
            BuildUpWater.Play();           
        }

        if (delay < 0)
        {
            if(up)
            {
                virtualAudioSource.enabled = false;
            } else
            {
                virtualAudioSource.enabled = true;
            }
            up = !up;
            delay = timer;
            BuildUpWater.Stop();
            if (!up)
            {
                MHCover.SetBool("Rumble", false);
            }
        }

        if (up)
        {
            waterCol.GetComponent<KnockBack>().KBForce = 10;

            for (int i = 0; i < water.Length; i++)
            {
                water[i].Play();
            }
            //float height = length/1.5f;
            //water.transform.localScale = Vector3.Lerp(water.transform.localScale, new Vector3(1, size, 1), Time.deltaTime * speed);
            waterCol.SetActive(true);
            cover.transform.localPosition = Vector3.Lerp(cover.transform.localPosition, coverTargetOffset,Time.deltaTime * speed);
        }
        else
        {
            //water.transform.localScale = Vector3.Lerp(water.transform.localScale, new Vector3(1, 1, 1), Time.deltaTime * speed); 
            for (int i = 0; i < water.Length; i++)
            {
                water[i].Stop();
            }
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverBaseT, Time.deltaTime * speed);
            waterCol.SetActive(false);
            waterCol.GetComponent<KnockBack>().KBForce = 0;
            

        }


    }

}
