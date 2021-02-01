using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FireHydrantKB : MonoBehaviour
{
    public bool Spraying = false;
    public float delay;
    public float length;
    private float timer;
    public short force;

    public GameObject waterKB;
    public ParticleSystem WaterEffect;
    private VirtualAudioSource spraySound;

    // Start is called before the first frame update
    void Start()
    {
        spraySound = GetComponent<VirtualAudioSource>();
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if ((delay > 0 && delay < 1) && Spraying == false)
        {
            
            var tmp = WaterEffect.main;
            tmp.startLifetime = .09f;
            WaterEffect.Play();

        }
        else if (delay < 0 && Spraying == true)
        {
            WaterEffect.Stop();
        }

        if (delay < 0)
        {
            if (Spraying)
                spraySound.enabled = false;
            else
                spraySound.enabled = true;
            Spraying = !Spraying;
            delay = timer;
        }

        if (Spraying)
        {
            //WaterEffect.Play();
            var tmp = WaterEffect.main;
            tmp.startLifetime = 1.45f;
            waterKB.SetActive(true);
            waterKB.GetComponent<KnockBack>().kBForce = force;
            
            

        }
        else
        {
            waterKB.SetActive(false);
            waterKB.GetComponent<KnockBack>().kBForce = 0;
            //WaterEffect.Stop();
        }
    }
}
