using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.EventSystems;

public class GraphicsOptionsMenu : MonoBehaviour
{
    public Toggle fullscreenToggle, vsyncToggle;

    public List<ResItem> resolutions = new List<ResItem>();
    private int selectedResolution;
    
    public List<GraphicsItem> graphicsOptions = new List<GraphicsItem>();
    private int selectedGraphicsOption = 3;

    public TMP_Text resolutionLabel;
    public TMP_Text graphicsLabel;

    public AK.Wwise.Event PlayButtonSoundEvent;
    public AudioMixer audioMixer;

    void Start()
    {
        fullscreenToggle.isOn = Screen.fullScreen;

        if(QualitySettings.vSyncCount == 0)
        {
            vsyncToggle.isOn = false;
        } else
        {
            vsyncToggle.isOn = true;
        }

        bool foundRes = false;
        for(int i = 0; i < resolutions.Count; i++)
        {
            if(Screen.width == resolutions[i].horizontal && Screen.height == resolutions[i].vertical)
            {
                foundRes = true;

                selectedResolution = i;

                UpdateResolutionLabel();
            }
        }

        if(!foundRes)
        {
            ResItem newRes = new ResItem();
            newRes.horizontal = Screen.width;
            newRes.vertical = Screen.height;

            resolutions.Add(newRes);
            selectedResolution = resolutions.Count - 1;

            UpdateResolutionLabel();
        }

        QualitySettings.SetQualityLevel(selectedGraphicsOption);
        UpdateGraphicsLabel();
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenToggle.isOn);
    }

    public void ResolutionLeft()
    {
        selectedResolution--;
        if(selectedResolution < 0)
        {
            selectedResolution = 0;
        }

        UpdateResolutionLabel();
    }

    public void ResolutionRight()
    {
        selectedResolution++;
        if(selectedResolution > resolutions.Count - 1)
        {
            selectedResolution = resolutions.Count - 1;
        }

        UpdateResolutionLabel();
    }

    public void UpdateResolutionLabel()
    {
        resolutionLabel.text = resolutions[selectedResolution].horizontal.ToString() + " x " + resolutions[selectedResolution].vertical.ToString();
    }

    public void GraphicsLeft()
    {
        selectedGraphicsOption--;
        if(selectedGraphicsOption < 0)
        {
            selectedGraphicsOption = 0;
        }

        //QualitySettings.SetQualityLevel(selectedGraphicsOption);

        UpdateGraphicsLabel();

    }

    public void GraphicsRight()
    {
        selectedGraphicsOption++;
        if(selectedGraphicsOption > graphicsOptions.Count - 1)
        {
            selectedGraphicsOption = graphicsOptions.Count - 1;
        }

        //QualitySettings.SetQualityLevel(selectedGraphicsOption);

        UpdateGraphicsLabel();
    }

    public void UpdateGraphicsLabel()
    {
        graphicsLabel.text = graphicsOptions[selectedGraphicsOption].setting.ToString();
    }

    public void ApplyGraphics()
    {
        QualitySettings.SetQualityLevel(selectedGraphicsOption);
        
        //Screen.fullScreen = fullscreenToggle.isOn;
        
        Screen.SetResolution(resolutions[selectedResolution].horizontal, resolutions[selectedResolution].vertical, fullscreenToggle.isOn);

        if(vsyncToggle.isOn)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

        public void PlayButtonSound()
    {
        GameObject Manager = GameObject.Find("Music Manager");
        PlayButtonSoundEvent.Post(Manager);
    }

}


[System.Serializable]
public class ResItem
{
    public int horizontal, vertical;
}

[System.Serializable]
public class GraphicsItem
{
    public string setting;
}
