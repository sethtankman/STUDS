using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public int target;
    // Start is called before the first frame update
    void Start()
    {
        //Application.targetFrameRate = target;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != Application.targetFrameRate)
        {
            Application.targetFrameRate = target;
        }
    }
}
