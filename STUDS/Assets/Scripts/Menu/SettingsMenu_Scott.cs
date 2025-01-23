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
    public TMP_Dropdown graphicsDropdown;

    public Slider volumeSlider; // Reference to the volume slider

    [SerializeField] private float defaultVolume = -1f; // Default volume in decibels

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

        // Set default volume
        float savedVolume = PlayerPrefs.GetFloat("volume", defaultVolume); // Load saved volume or use default
        audioMixer.SetFloat("volume", savedVolume);

        // Synchronize slider with the saved/default volume
        if (volumeSlider != null)
        {
            volumeSlider.value = Mathf.InverseLerp(-80f, 0f, savedVolume); // Slider value (0 to 1)
        }

        resolutions = Screen.resolutions;

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
            resolutionDropdown.AddOptions(options);

            UpdateDropdownValues();
        }

        if (graphicsDropdown != null)
        {
            graphicsDropdown.value = QualitySettings.GetQualityLevel();
            graphicsDropdown.RefreshShownValue();
        }
    }

    private void OnEnable()
    {
        GameObject pv2 = GameObject.Find("GameManager");
        GameObject ngm = GameObject.Find("NetGameManager");
        if (pv2)
        {
            menuPlayButton.GetComponent<Button>().onClick.AddListener(pv2.GetComponent<PauseV2>().Pause);
        }
        else if (ngm)
        {
            menuPlayButton.GetComponent<Button>().onClick.AddListener(ngm.GetComponent<NetPause>().Pause);
        }
    }

    public void SetResolution(int _resolutionIndex)
    {
        resolutionIndex = _resolutionIndex * numRefreshOptions;
        if (resolutions.Length > resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    public void SetVolume(float sliderValue)
    {
        // Convert slider value (0 to 1) to decibels (-80 to 0)
        float volume = Mathf.Lerp(-80f, 0f, sliderValue);
        audioMixer.SetFloat("volume", volume);

        // Save volume setting
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
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
