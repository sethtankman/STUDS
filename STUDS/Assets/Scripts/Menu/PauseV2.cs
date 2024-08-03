using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseV2 : MonoBehaviour
{
    public static bool canPause = true;
    public static bool gameisPaused = false;

    /// <summary>
    /// Relies on other scripts to tell us where the PauseMenuUI is
    /// </summary>
    public GameObject PauseMenuUI;
    public GameObject MainPauseMenu;
    public GameObject firstButton;
    public GameObject[] allOtherMenus;
    public GameObject p4PH;
    public bool p4PHWasEnabled;

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
        p4PH = GameObject.Find("Player4PlaceHolder");
        if (p4PH == false)
            Debug.Log("PauseV2 Couldn't find Player 4 PlaceHolder");
        gameisPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(p4PH && ManagePlayerHub.Instance)
            ManagePlayerHub.Instance.player4PlaceHolder = p4PH;
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
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if(p4PH && p4PH.activeSelf)
            p4PHWasEnabled = true;
        if (!gameisPaused)
        {
            if(p4PH) // Activates the player 4 placeholder image if it is set.
                p4PH.SetActive(false);
            if (EventSystem.current)
            {
                //EventSystem.current.SetSelectedGameObject(null);
                //EventSystem.current.SetSelectedGameObject(firstButton);
            }
            else
            {
                Debug.LogError("Event System not found.");
            }
            gameisPaused = true;
            Time.timeScale = 0f;
            PauseMenuUI.SetActive(true);
            allOtherMenus = PauseMenuUI.GetComponent<SettingsMenu_Scott>().allOtherMenus;
            MainPauseMenu = PauseMenuUI.GetComponent<SettingsMenu_Scott>().GetMainPauseMenu();
            MainPauseMenu.SetActive(true);
        }
        else
        {
            if (p4PHWasEnabled)
                p4PH.SetActive(true);
            DeactivateAll();
            gameisPaused = false;
            Time.timeScale = 1f;
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
        MainPauseMenu.SetActive(true);
        foreach(GameObject menu in allOtherMenus)
        {
            menu.SetActive(false);
        }
    }

    void OnEnable()
    {
        thePause.Menu.Enable();
    }
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        thePause.Menu.Disable();
    }

}