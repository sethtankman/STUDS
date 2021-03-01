using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KidTimeout : MonoBehaviour
{
    public double timeoutTimer;
    private double currTime;
    private bool isTimeout;
    public Vector3 timeoutPos;
    public Vector3 backinPos;
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
                currTime = 0;
                isTimeout = false;
                gameObject.transform.position = backinPos;
            }
            else
            {
                currTime += Time.deltaTime;
            }
        }
    }

    public void Timeout()
    {
        gameObject.transform.position = timeoutPos;
        isTimeout = true;
    }
}
