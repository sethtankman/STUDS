using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXVolume : MonoBehaviour
{
    public Slider thisSlider;
    public float SFX;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = thisSlider.value;
        Debug.Log(sliderValue);

        if (whatValue == "SFX")
        {
            //Debug.Log("changed master volume to :" + thisSlider.value);
            SFX = thisSlider.value;
            AkSoundEngine.SetRTPCValue("SFX", SFX);

        }
    }
}
