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
    public AK.Wwise.Event spraySound;

    // Start is called before the first frame update
    void Start()
    {
        timer = delay;
    }

    // Update is called once per frame
    void Update()
    {
        delay -= Time.deltaTime;

        if ((delay > 0 && delay < 1) && Spraying == false)
        {
            SteamSpray.Play();

        }

        if (delay < 0)
        {
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
            if(waterKB.GetComponent<KnockBack>())
                waterKB.GetComponent<KnockBack>().KBForce = force;
            else
                waterKB.GetComponent<NetKnockBack>().KBForce = force;
        }
        else
        {
            WaterEffect.Stop();
            waterKB.SetActive(false);
            if (waterKB.GetComponent<KnockBack>())
                waterKB.GetComponent<KnockBack>().KBForce = 0;
            else
                waterKB.GetComponent<NetKnockBack>().KBForce = 0;
        }
    }
}
