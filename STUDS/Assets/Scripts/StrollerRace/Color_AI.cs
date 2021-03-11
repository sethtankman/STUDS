using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_AI : MonoBehaviour
{
    public string ToColor;

    // Start is called before the first frame update
    void Awake()
    {
        GetComponent<CharacterMovementController>().SetColorName(ToColor);
    }

}
