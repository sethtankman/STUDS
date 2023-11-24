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
            Arrow.transform.LookAt(StrollerTarget.transform);
        }
    }

    public void SetActive(bool tf)
    {
        Arrow.SetActive(tf);
    }


    public void PassStrollerID(GameObject Stroller)
    {
        StrollerTarget = Stroller;
        Linked = true;
        Arrow.SetActive(true);
    }

    public void ResetArrow()
    {
        StrollerTarget = null;
        Arrow.SetActive(false);
    }

    public bool HasLink()
    {
        return Linked;
    }
}
