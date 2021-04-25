using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class KidTimeout : MonoBehaviour
{
    public double timeoutTimer;
    private double currTime;
    private bool isTimeout;
    GameObject mini;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isTimeout)
        {
            if(currTime > timeoutTimer)
            {
                GameObject pos = GameObject.Find("KidTimeoutBackInside");
                gameObject.transform.position = pos.transform.position;
                // We need to reset the navigation agent when we teleport it as well.
                if (currTime > (timeoutTimer + 0.1))
                {
                    currTime = 0;
                    mini.GetComponent<CharacterMovementController>().CanMove = true;
                    if (mini.GetComponent<CharacterMovementController>().isAI)
                    {
                        mini.GetComponent<NavMeshAgent>().Warp(pos.transform.position);
                        mini.GetComponent<PennyPincherAI>().CanMove = true;
                    }
                    mini = null;
                    isTimeout = false;
                    Debug.Log("Back in");
                }
                currTime += Time.deltaTime;
            }
            else
            {
                currTime += Time.deltaTime;
                GameObject pos = GameObject.Find("KidTimeoutOutside");
                gameObject.transform.position = pos.transform.position;
            }
        }
    }

    public void Timeout(GameObject mini)
    {
        Debug.Log("in timeout");
        GameObject pos = GameObject.Find("KidTimeoutOutside");
        gameObject.transform.position = pos.transform.position;
        mini.GetComponent<CharacterMovementController>().CanMove = false;
        if (mini.GetComponent<CharacterMovementController>().isAI)
        {
            mini.GetComponent<PennyPincherAI>().CanMove = false;
        }
        this.mini = mini;
        isTimeout = true;
    }
}
