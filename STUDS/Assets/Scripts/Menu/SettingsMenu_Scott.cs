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

        if (!resolutionDropdown)
            return;
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
        SetResolution(i - 1);
    }

    /// <summary>
    /// Because Pausing and unpausing requires a pauseV2 object, which is a persistent object across scenes.
    /// This finds the game manager and adds it to each scene's pause button listeners.
    /// </summary>
    private void OnEnable()
    {
        GameObject pv2 = GameObject.Find("GameManager");
        GameObject ngm = GameObject.Find("NetGameManager");
        if (pv2)
        {
            menuPlayButton.GetComponent<Button>().onClick.AddListener(pv2.GetComponent<PauseV2>().Pause);
        } else if (ngm) {
            menuPlayButton.GetComponent<Button>().onClick.AddListener(ngm.GetComponent<NetPause>().Pause);
        }
    }

    public Dictionary<string, int> val = new Dictionary<string, int>();

    public void SetResolution(int _resolutionIndex)
    {
        resolutionIndex = _resolutionIndex * numRefreshOptions;
        if (resolutions.Length > resolutionIndex)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
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
