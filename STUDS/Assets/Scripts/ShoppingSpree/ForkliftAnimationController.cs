using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForkliftAnimationController : MonoBehaviour
{
    public float timer;
    public Animator _animator;
    public bool Arrows_Forklift;
    public GameObject UpArrow, DownArrow;

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
            Arrows_Forklift = !Arrows_Forklift;
            if (Arrows_Forklift == false)
            {
                DownArrow.SetActive(false);
            }
            else
            {
                UpArrow.SetActive(false);
            }
            StartCoroutine("WaitForArrow");
        }
    }

    private IEnumerator WaitForArrow()
    {
        yield return new WaitForSeconds(5f);
        if (Arrows_Forklift == false)
        {
            UpArrow.SetActive(true);
        }
        else
        {
            DownArrow.SetActive(true);
        }
    }
}
