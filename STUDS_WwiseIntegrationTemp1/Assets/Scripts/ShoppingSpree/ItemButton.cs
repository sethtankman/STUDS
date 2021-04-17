using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public GameObject checkMark;

    public void EnableCheckMark()
    {
        checkMark.SetActive(true);
    }
}
