using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_AI : MonoBehaviour
{
    public string ToColor;
    public CharacterMovementController CMC;
    public NetworkCharacterMovementController NCMC;

    // Start is called before the first frame update
    void Awake()
    {
        if (CMC)
            CMC.SetColorName(ToColor);
        else
            NCMC.SetColorName(ToColor);
    }

}
