using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Propeller : MonoBehaviour
{
    public float turnSpeed = 0;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(turnSpeed * Time.deltaTime, 0, 0, Space.Self);
    }
}
