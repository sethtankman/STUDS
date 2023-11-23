using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhoIsDisabling : MonoBehaviour
{
    private void OnEnable()
    {
        Debug.Log("Who enabled me?");
    }

    private void OnDisable()
    {
        Debug.Log("Who disabled me?");
    }
}
