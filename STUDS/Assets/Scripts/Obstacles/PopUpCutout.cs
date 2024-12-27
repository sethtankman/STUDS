using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class PopUpCutout : MonoBehaviour
{
    public Quaternion currentPos;
    public Quaternion rotationAngle;
    public float speed;
    public bool Standing;
    public float timer;
    private float countdown;
    private short initialForce;

    // Start is called before the first frame update
    void Start()
    {
        countdown = timer;
        if (GetComponent<KnockBack>())
            initialForce = GetComponent<KnockBack>().KBForce;
        else
            initialForce = GetComponent<NetKnockBack>().KBForce;
        currentPos = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
         countdown -= Time.deltaTime;
        if (countdown < 0) {
            Standing = !Standing;
            countdown = timer;
        }

        if (Standing)
        {
            if (GetComponent<KnockBack>())
                GetComponent<KnockBack>().KBForce = 0;
            else
                GetComponent<NetKnockBack>().KBForce = 0;
            GetComponent<BoxCollider>().enabled = false;
            Quaternion wantedRotation = transform.rotation * Quaternion.AngleAxis(10, Vector3.left);
            transform.rotation = Quaternion.Slerp(transform.rotation, wantedRotation, Time.deltaTime*speed);
        }
        else
        {
            if (GetComponent<KnockBack>())
                GetComponent<KnockBack>().KBForce = initialForce;
            else
                GetComponent<NetKnockBack>().KBForce = initialForce;
            GetComponent<BoxCollider>().enabled = true;
            transform.rotation = Quaternion.Slerp(transform.rotation, currentPos, Time.deltaTime * speed);
        }

    }

}
