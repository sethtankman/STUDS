using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayAnimation : MonoBehaviour
{
    public float timeToDelay = 0.4f;
    public GameObject animatedObject;

    void Start()
    {
        Invoke("EnableAnimator", timeToDelay);
    }

    void EnableAnimator()
    {
        animatedObject.GetComponent<Animator>().enabled = true;
    }

}
