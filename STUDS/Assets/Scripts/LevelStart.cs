﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelStart : MonoBehaviour
{
    public GameObject startCam;
    public GameObject pauseMenuUI;

    public Text startText;

    private List<PlayerConfiguration> PlayerConfigs;

    public Material playerColor1;
    public Material playerColor2;
    public Material playerColor3;
    public Material playerColor4;

    private int playerIDCount = 0;

    public void Start()
    {
        if (pauseMenuUI)
            GameObject.Find("GameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        if(NetGameManager.Instance) // LevelStart is used on local too.
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player"))
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(false);
    }

    public void HandleGameStart(PlayerInput pi)
    {
        if(startCam != null)
        {
            Destroy(startCam);
            startText.text = "";
        }
        if (pi.gameObject.GetComponent<CharacterMovementController>())
        {
            pi.gameObject.GetComponent<CharacterMovementController>().SetPlayerID(playerIDCount);
            if(playerIDCount == 0)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor1;
            }
            else if (playerIDCount == 1)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor2;
            }
            else if (playerIDCount == 2)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor3;
            }
            else if (playerIDCount == 3)
            {
                pi.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor4;
            }

        }

        playerIDCount++;

    }

    public void DisableJoin()
    {
        PlayerInputManager.instance.DisableJoining();
    }
}

