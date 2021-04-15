using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCPTracker : MonoBehaviour
{
    public GameObject camera;
    public bool obstructed;

    // Update is called once per frame
    void Update()
    {
        if (obstructed)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out hit, 3.5f))
            {
                if (hit.transform.tag == "Player")
                {
                    camera.transform.position = transform.position;
                    obstructed = false;
                }
            }
        }
    }
}
