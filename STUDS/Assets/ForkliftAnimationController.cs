using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftAnimationController : MonoBehaviour
{
    public float timer;
    public Animator _animator;

    private void Update()
    {
        if (timer < 0)
        {
            // timer has run out = true
            _animator.SetBool("timerHasRunOut", true);

        }
        else
        {
            timer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player" && timer < 0)
        {
            _animator.SetTrigger("hasEntered");
            timer = 5f;
        }
    }
}
