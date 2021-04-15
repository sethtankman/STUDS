using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonSound : MonoBehaviour
{
    public void onClick()
    {
        AkSoundEngine.PostEvent("Menu_Select", gameObject);
    }
}
