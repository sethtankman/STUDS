using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Mirror;

/// <summary>
/// The level initializer for the stroller race levels.
/// </summary>
public class NetDBInit : NetworkBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    private bool spawnPlayers = false;
    public float waitTime = 5.0f;
    private float currentTime = 0;

    [SerializeField] private Material[] materials;

    private GameObject[] players;
    private List<string> aiColors;

    //public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject startCam;
    public GameObject startUI;

    public TextMeshProUGUI startText;
    [SerializeField] private NetDBUI ui;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", true);
        aiColors = new List<string> { "red", "blue", "purple", "yellow", "green" };
        PauseV2.canPause = false;
        if (NetGameManager.Instance) 
        {
            NetGameManager.Instance.GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
        } else
            Debug.LogWarning("Manage Player Hub not found!");
        PlayerInputManager.instance.DisableJoining();
        Invoke("LateStart", 0.5f);

    }

    private void LateStart()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            aiColors.Remove(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
            player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
        }
        if (players != null)
        {
            for (int i = 0; i < players.Length; i++)
            {
                //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                //players[i].transform.forward = new Vector3(0, 0, 1);
                players[i].transform.position = playerSpawns[i].position;
                players[i].transform.rotation = playerSpawns[i].rotation;
            }
        }
        spawnPlayers = true;

    }

    // Update is called once per frame
    void Update()
    {

        currentTime += Time.deltaTime;
        if (currentTime > waitTime && spawnPlayers)
        {
            Destroy(startCam);
            Destroy(startText);
            Destroy(startUI);
            //startText.text = "";
            if (players != null)
            {
                for (int i = 0; i < players.Length; i++)
                {
                    if(players[i].GetComponent<NetworkCharacterMovementController>().isAI)
                    {
                        string aiColor = aiColors[0];
                        players[i].GetComponentInChildren<NetworkCharacterMovementController>(true).SetColorName(aiColor);
                        players[i].GetComponentInChildren<SkinnedMeshRenderer>(true).material = materials[GetColorIndex(aiColor)];
                        aiColors.Remove(aiColor);
                    }
                    spawnPlayers = false;
                }
                ui.UpdateSpriteColors();
                NetDBGameManager.Instance.InitScores();
            }
            PauseV2.canPause = true;
        }
    }

    private int GetColorIndex(string _color)
    {
        switch (_color)
        {
            case "red":
                return 0;
            case "blue":
                return 1;
            case "purple":
                return 2;
            case "yellow":
                return 3;
            case "green":
                return 4;
            default:
                Debug.LogError("Invalid color name");
                return -1;
        }
    }
}
