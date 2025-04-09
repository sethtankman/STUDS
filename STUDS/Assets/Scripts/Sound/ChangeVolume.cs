using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeVolume : MonoBehaviour
{
    public Slider thisSlider;
    public float masterVolume;
    // Start is called before the first frame update
    void Start()
    {
        masterVolume = PlayerPrefs.GetFloat("Volume", 50.0f);
        thisSlider.value = masterVolume;
    }
    
    public void SetSpecificVolume(string whatValue)
    {
        float sliderValue = thisSlider.value;
        if (whatValue == "Master")
        {
            masterVolume = thisSlider.value;
            PlayerPrefs.SetFloat("Volume", thisSlider.value);
            AkUnitySoundEngine.SetRTPCValue("Master", masterVolume);
        }
    }
}
