using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SettingsMenu_Scott : MonoBehaviour
{
    public AudioMixer audioMixer;

    public static bool GameIsPaused = false;

    Resolution[] resolutions;

    private int resolutionIndex, numRefreshOptions;

    public TMP_Dropdown resolutionDropdown;

    public GameObject menuPlayButton, optionsFirstButton, optionsCloseButton, quitFirstButton, creditsFirstButton,
        extrasFirstButton, feedbackFirstButton, videoFirstButton, videoCloseButton, soundFirstButton, soundCloseButton,
        controlsFirstButton, controlsCloseButton, ReturnFirstButton, okayButton, noKickButton;

    void Start()
    {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        string option = "";
        int i = 0;
        while (i < resolutions.Length)
        {
            if (option != resolutions[i].width + "x" + resolutions[i].height)
            {
                option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);
            }
            i++;
        }

        numRefreshOptions = i / options.Count;
        resolutionDropdown.AddOptions(options);
        SetResolution(i-1);
    }

    private void Update()
    {

    }

    public Dictionary<string, int> val = new Dictionary<string, int>();

    public void SetResolution(int _resolutionIndex)
    {
        resolutionIndex = _resolutionIndex * numRefreshOptions;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetRefresh(int refreshRate)
    {
        int i = resolutionIndex;
        while (i < resolutions.Length)
        {
            if (resolutions[i].refreshRate == refreshRate)
            {
                Debug.Log("Refresh Rate set to: " + resolutions[i].refreshRate);
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreen, resolutions[i].refreshRate);
            }
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void OpenMainMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuPlayButton);
    }

    public void OpenOptionsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void CloseOptionsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuPlayButton);
    }

    public void OpenVideoMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(videoFirstButton);
    }

    public void CloseVideoMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void OpenSoundMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(soundFirstButton);
    }

    public void OpenPlayerConnectionMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(okayButton);
    }

    public void CloseSoundMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void OpenControlsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsFirstButton);
    }

    public void CloseControlsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(optionsFirstButton);
    }

    public void OpenCreditsMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(creditsFirstButton);
    }

    public void OpenExtrasMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(extrasFirstButton);
    }

    public void OpenFeedbackMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(feedbackFirstButton);
    }

    public void OpenQuitMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(quitFirstButton);
    }

    public void OpenURL()
    {
        Application.OpenURL("https://discord.gg/MvrPPpy6");
    }

    public void OpenReturnMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ReturnFirstButton);
    }

    public void OpenKickPlayerDialogue()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(noKickButton);
    }
}
