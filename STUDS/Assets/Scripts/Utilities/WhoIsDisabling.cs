using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script for debugging objects that are mysteriously enabled or disabled.
/// </summary>
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
