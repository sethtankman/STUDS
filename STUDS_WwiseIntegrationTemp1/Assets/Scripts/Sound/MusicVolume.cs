using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicVolume : MonoBehaviour
{
    public Slider thisSlider;
    public float MX;
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

        if (whatValue == "Music")
        {
            //Debug.Log("changed master volume to :" + thisSlider.value);
            MX = thisSlider.value;
            AkSoundEngine.SetRTPCValue("MX", MX);

        }
    }
}
