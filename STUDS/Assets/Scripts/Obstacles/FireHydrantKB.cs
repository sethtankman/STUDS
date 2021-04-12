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
    public ParticleSystem SteamSpray;
    //private VirtualAudioSource spraySound;
    public AK.Wwise.Event spraySound;

    // Start is called before the first frame update
    void Start()
    {
        //spraySound = GetComponent<VirtualAudioSource>();
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if ((delay > 0 && delay < 1) && Spraying == false)
        {
            
            //var tmp = WaterEffect.main;
            //tmp.startLifetime = .09f;
            SteamSpray.Play();

        }

        if (delay < 0)
        {
            /*
            if (Spraying)
                spraySound.enabled = false;
            else
                spraySound.enabled = true;
            */
            Spraying = !Spraying;
            delay = timer;
            SteamSpray.Stop();
        }

        if (delay < 0 && Spraying == true)
        {

            
        }

        

        if (Spraying)
        {
            WaterEffect.Play();

            waterKB.SetActive(true);
            waterKB.GetComponent<KnockBack>().KBForce = force;
            
            

        }
        else
        {
            WaterEffect.Stop();
            waterKB.SetActive(false);
            waterKB.GetComponent<KnockBack>().KBForce = 0;
            //WaterEffect.Stop();
        }
    }
}
