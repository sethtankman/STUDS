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

    public Toggle vSyncToggle;
    private bool isVSyncOn = false;

    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown graphicsDropdown;

    public GameObject[] allOtherMenus;
    public GameObject menuPlayButton, optionsFirstButton, optionsCloseButton, quitFirstButton, creditsFirstButton,
        extrasFirstButton, feedbackFirstButton, videoFirstButton, videoCloseButton, soundFirstButton, soundCloseButton,
        controlsFirstButton, controlsCloseButton, ReturnFirstButton, okayButton, noKickButton, onlineFirstButton;

    [SerializeField] private GameObject mainPauseMenu;

    public AK.Wwise.Event PlayButtonSoundEvent;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        resolutions = Screen.resolutions;

        QualitySettings.vSyncCount = 0;
        isVSyncOn = false;

        // Update the toggle to match the V-Sync state
        if (vSyncToggle != null)
        {
            vSyncToggle.isOn = isVSyncOn;
            vSyncToggle.onValueChanged.AddListener(OnVSyncToggleChanged);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();
            string lastOption = "";

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = $"{resolutions[i].width}x{resolutions[i].height}";
                if (option != lastOption)
                {
                    options.Add(option);
                    lastOption = option;
                }
            }

            numRefreshOptions = resolutions.Length / options.Count;
            Debug.Log($"Resolutions: {resolutions.Length}, Options: {options.Count}, NumRefreshOptions: {numRefreshOptions}");
            resolutionDropdown.AddOptions(options);

            UpdateDropdownValues();
            SetResolution(resolutionDropdown.value); // Set the initial resolution
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
            graphicsDropdown.RefreshShownValue();
        }

        // Ensure the game starts in fullscreen mode
        Screen.fullScreen = true;

        // Force the camera's aspect ratio to 16:9
        Camera.main.aspect = 16f / 9f;
    }

    private void UpdateDropdownValues()
    {
        // Set current resolution in the dropdown
        if (resolutionDropdown != null)
        {
            Resolution currentResolution = Screen.currentResolution;
            for (int i = 0; i < resolutions.Length; i++)
            {
                if (resolutions[i].width == currentResolution.width &&
                    resolutions[i].height == currentResolution.height)
                {
                    resolutionDropdown.value = i / numRefreshOptions;
                    resolutionDropdown.RefreshShownValue();
                    break;
                }
            }
        }
    }

    public void SetResolution(int _resolutionIndex)
    {
        resolutionIndex = _resolutionIndex * numRefreshOptions;
        if (resolutions.Length > resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Debug.Log($"Setting resolution to: {resolution.width}x{resolution.height}");
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
        else
        {
            Debug.LogError($"Resolution index out of range: {resolutionIndex}");
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

    public void OnVSyncToggleChanged(bool isOn)
    {
        // Update V-Sync state when the toggle is changed
        isVSyncOn = isOn;
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;

        //Debug.Log("QualitySettings.vSyncCount is now: " + QualitySettings.vSyncCount);
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

    public void OpenOnlineMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(onlineFirstButton);
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

    public void CloseOnlineMenu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuPlayButton);
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

    public void PlayButtonSound()
    {
        GameObject Manager = GameObject.Find("Music Manager");
        PlayButtonSoundEvent.Post(Manager);
    }

    public GameObject GetMainPauseMenu()
    {
        return mainPauseMenu;
    }
}