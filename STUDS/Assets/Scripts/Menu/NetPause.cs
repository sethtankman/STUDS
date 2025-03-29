using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Replaces PauseV2 for the network
/// </summary>
public class NetPause : MonoBehaviour { 
    public static bool canPause = true;
    public static bool gameisPaused = false;

    /// <summary>
    /// Relies on other scripts to tell us where the PauseMenuUI is
    /// </summary>
    public GameObject PauseMenuUI;
    public GameObject MainPauseMenu;
    public GameObject firstButton;
    public GameObject[] allOtherMenus;

    public PauseMenu thePause;

    public Text ResumeText;
    public Text MenuText;
    public Text OptionsText;
    public Text QuitText;

    public int newFontsize01;
    public int newFontsize02;

    public bool oneCycle = false;

    private void OnLevelWasLoaded(int level)
    {
        gameisPaused = false;
    }
    
    void Awake()
    {
        thePause = new PauseMenu();

        thePause.Menu.Pause.performed += ctx => Pause();

    }



    public void Pause()
    {
        if (canPause == false)
        {
            Debug.Log("CanPause is false");
            return;
        }
        if (!gameisPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            gameisPaused = true;
            foreach (NetworkCharacterMovementController player in FindObjectsByType<NetworkCharacterMovementController>(FindObjectsSortMode.None))
                player.CanMove = false;
            PauseMenuUI.SetActive(true);
            allOtherMenus = PauseMenuUI.GetComponent<SettingsMenu_Scott>().allOtherMenus;
            MainPauseMenu = PauseMenuUI.GetComponent<SettingsMenu_Scott>().GetMainPauseMenu();
            MainPauseMenu.SetActive(true);
        }
        else
        {
            DeactivateAll();
            gameisPaused = false;
            foreach (NetworkCharacterMovementController player in FindObjectsByType<NetworkCharacterMovementController>(FindObjectsSortMode.None))
                player.CanMove = true;
        }
    }

    /// <summary>
    /// Used to deactivate all menus when exiting the pause menu.
    /// </summary>
    private void DeactivateAll()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PauseMenuUI.SetActive(false);
        if(!MainPauseMenu)
            MainPauseMenu = PauseMenuUI.GetComponent<SettingsMenu_Scott>().GetMainPauseMenu();
        MainPauseMenu.SetActive(true);
        foreach(GameObject menu in allOtherMenus)
        {
            if(menu)
                menu.SetActive(false);
        }
    }

    void OnEnable()
    {
        thePause.Menu.Enable();
    }

    void OnDisable()
    {
        thePause.Menu.Disable();
    }

}