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

    public GameObject PauseMenuUI;
    public GameObject OptionsMenu;
    public GameObject firstButton;
    public GameObject[] allOtherMenus;
    public GameObject p4PH;
    public bool p4PHWasEnabled;

    public GameObject theImage;
    public GameObject theImage01;
    public GameObject theImage02;
    public GameObject theImage03;

    int menutracker = 0;


    public PauseMenu thePause;

    public Text ResumeText;
    public Text MenuText;
    public Text OptionsText;
    public Text QuitText;

    public int newFontsize01;
    public int newFontsize02;

    public bool oneCycle = false;

    private bool lostPausePanels = false;

    private void OnLevelWasLoaded(int level)
    {
        lostPausePanels = true;
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

        //p4PH = GameObject.Find("P4PlaceHolder");

        //thePause.Menu.Return.performed += ctx => Return();

        //thePause.Menu.ScrollDown.performed += ctx => ScrollDown();

        //thePause.Menu.ScrollUp.performed += ctx => ScrollUp();

        //thePause.Menu.Go.performed += ctx => Go();

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
        //Debug.Log($"GameIsPaused: {gameisPaused}, LostPausePanels: {lostPausePanels}");
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
            if (lostPausePanels)
            {
                allOtherMenus = PauseMenuUI.GetComponent<SettingsMenu_Scott>().allOtherMenus;
                OptionsMenu = GameObject.Find("PauseMenu");
            }

            OptionsMenu.SetActive(true);
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
        OptionsMenu.SetActive(true);
        foreach(GameObject menu in allOtherMenus)
        {
            menu.SetActive(false);
        }
    }



    void Return()
    {
        if (gameisPaused)
        {
            PauseMenuUI.SetActive(false);

            gameisPaused = false;
            Time.timeScale = 1f;
        }
    }

    void ScrollDown()
    {


        if (menutracker == 0)
        {

            theImage.SetActive(false);
            theImage01.SetActive(true);

        }

        else if (menutracker == 1)
        {

            theImage01.SetActive(false);
            theImage02.SetActive(true);
        }

        else if (menutracker == 2)
        {

            theImage02.SetActive(false);
            theImage03.SetActive(true);

        }

        else if (menutracker == 3)
        {
            theImage03.SetActive(false);
            theImage.SetActive(true);
            menutracker = -1;
        }


        menutracker++;
    }

    void ScrollUp()
    {
        if (menutracker == 1)
        {
            theImage.SetActive(true);
            theImage01.SetActive(false);
            menutracker = 0;
        }

        else if (menutracker == 2)
        {
            theImage01.SetActive(true);
            theImage02.SetActive(false);

            menutracker = 1;
        }

        else if (menutracker == 3)
        {
            theImage02.SetActive(true);
            theImage03.SetActive(false);
            menutracker = 2;
        }


        else if (menutracker == 0)
        {
            theImage03.SetActive(true);
            theImage.SetActive(false);
            menutracker = 3;

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