using System.Collections;
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
            if (waterCol.GetComponent<KnockBack>())
                waterCol.GetComponent<KnockBack>().KBForce = 10;
            else
                waterCol.GetComponent<NetKnockBack>().KBForce = 10;

            for (int i = 0; i < water.Length; i++)
            {
                water[i].Play();
            }
            waterCol.SetActive(true);
            cover.transform.localPosition = Vector3.Lerp(cover.transform.localPosition, coverTargetOffset,Time.deltaTime * speed);
        }
        else
        {
            for (int i = 0; i < water.Length; i++)
            {
                water[i].Stop();
            }
            cover.transform.position = Vector3.Lerp(cover.transform.position, coverBaseT, Time.deltaTime * speed);
            waterCol.SetActive(false);
            if (waterCol.GetComponent<KnockBack>())
                waterCol.GetComponent<KnockBack>().KBForce = 0;
            else
                waterCol.GetComponent<NetKnockBack>().KBForce = 0;
        }
    }
}
