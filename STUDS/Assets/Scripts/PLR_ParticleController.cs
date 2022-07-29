using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PLR_ParticleController : MonoBehaviour
{
    //For the dust cloud
    public ParticleSystem Dustcloud;
    public bool IsRunning = false;
    public CharacterMovementController CC;
    public NetworkCharacterMovementController NCC;

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
        if (CC)
        {
            if (CC.isMoving)
            {
                TurnOnRunning();
            }
            else
            {
                TurnOffRunning();
            }
        } else
        {
            if (NCC.isMoving)
            {
                TurnOnRunning();
            } else
            {
                TurnOffRunning();
            }
        }

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
        if (other.GetComponentInParent<Interaction>())
            {
                Interaction InteractiveSC = other.GetComponentInParent<Interaction>();
                if (other.tag == "Electronics" && InteractiveSC.interactPressed == false && CC.isMini == false)
                {
                    inRange = true;
                    //Emote.SetActive(true);

                }
            if (other.tag == "Electronics" && InteractiveSC.interactPressed == true && CC.isMini == true)
            {
                inRange = true;
                //Emote.SetActive(true);

            }
            else inRange = false;
        }
        if (other.CompareTag("ShoppingItem"))
        {
            inRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Emote.SetActive(false);
        inRange = false;
    }
}
