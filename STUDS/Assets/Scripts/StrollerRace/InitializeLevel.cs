﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The level initializer for the stroller race levels.
/// </summary>
public class InitializeLevel : MonoBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    private bool spawnedPlayers = false;
    public float waitTime = 5f;
    private float currentTime = 0;

    public Material strollerColor1;
    public Material strollerColor2;
    public Material strollerColor3;
    public Material strollerColor4;
    public Material strollerColor5;

    private List<GameObject> players;

    public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject startCam;

    public TextMeshProUGUI startText;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", true);
        players = ManagePlayerHub.Instance.getPlayers();
        PlayerInputManager.instance.DisableJoining();
        if(pauseMenuUI)
            GameObject.Find("GameManager").GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
    }

    // Update is called once per frame
    void Update()
    {

        currentTime += Time.deltaTime;
        Debug.Log("curr time is: " + currentTime);
        if (currentTime > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            startText.text = "";
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    GameObject stroller = Instantiate(strollerPrefab, playerSpawns[i].position + new Vector3(0, 0, 2f), Quaternion.identity);
                    DetermineColor(players[i].GetComponent<CharacterMovementController>().GetColorName(), stroller);
                    stroller.GetComponent<StrollerController>().SetID(players[i].GetComponent<CharacterMovementController>().getPlayerID());
                    spawnedPlayers = true;
                }
            }
        }
        else if(!spawnedPlayers)
        {
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                    //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                    players[i].transform.forward = new Vector3(0, 0, 1);
                    players[i].transform.position = playerSpawns[i].position;
                }
            }
        }
    }

    private void DetermineColor(string colorName, GameObject stroller)
    {
        Debug.Log(colorName);
        if (colorName.Equals("blue"))
        {
            stroller.GetComponent<MeshRenderer>().material = strollerColor1;
        }
        else if (colorName.Equals("green"))
        {
            stroller.GetComponent<MeshRenderer>().material = strollerColor2;
        }
        else if (colorName.Equals("red"))
        {
            Debug.Log("Orange Stroller");
            stroller.GetComponent<MeshRenderer>().material = strollerColor3;
        }
        else if (colorName.Equals("yellow"))
        {
            stroller.GetComponent<MeshRenderer>().material = strollerColor4;
        }
        else if (colorName.Equals("purple"))
        {
            stroller.GetComponent<MeshRenderer>().material = strollerColor5;
        }
        stroller.GetComponent<StrollerController>().SetColor(colorName);
    }
}