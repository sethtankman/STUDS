using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_AI : MonoBehaviour
{
    public string ToColor;
    public CharacterMovementController CMC;

    // Start is called before the first frame update
    void Awake()
    {
        CMC.SetColorName(ToColor);
    }

}
