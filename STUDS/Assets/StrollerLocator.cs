using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrollerLocator : MonoBehaviour
{
    public GameObject Arrow;
    public bool Linked;
    public bool active = true;

    private GameObject StrollerTarget;

    


    // Start is called before the first frame update
    void Start()
    {
        ResetArrow();
    }

    // Update is called once per frame
    void Update()
    {
        if (StrollerTarget && Linked == true)
        {
            if (active)
            {
                Arrow.SetActive(true);
            }
            else
            {
                Arrow.SetActive(false);
            }
            //active = true;
            Arrow.transform.LookAt(StrollerTarget.transform, -Arrow.transform.forward);
        }
        if (StrollerTarget == null)
        {
            Arrow.SetActive(false);
            //active = false;

        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == StrollerTarget && active == true)
        {
            active = false;
           // Arrow.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == StrollerTarget && active == false)
        {
            active = true;
            //Arrow.SetActive(true);
        }
    }


    public void PassStrollerID(GameObject Stroller)
    {
        StrollerTarget = Stroller;
        Linked = true;
    }

    public void ResetArrow()
    {
        StrollerTarget = null;
    }

    public bool HasLink()
    {
        return Linked;
    }
}
