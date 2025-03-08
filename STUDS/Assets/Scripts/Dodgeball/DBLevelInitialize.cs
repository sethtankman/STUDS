using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The level initializer for the dodgeball levels.
/// </summary>
public class DBLevelInitialize : MonoBehaviour
{
    public Transform[] playerSpawns;
    public GameObject playerPrefab;

    private bool spawnedPlayers = false;
    public float waitTime = 5.0f;
    private float currentTime = 0;

    [SerializeField] private Material[] materials;

    private List<GameObject> players;
    private List<string> aiColors;

    //public GameObject strollerPrefab;
    public GameObject pauseMenuUI;
    public GameObject startCam;
    public GameObject startUI;

    public TextMeshProUGUI startText;
    [SerializeField] private DB_UI ui;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Dodgeball", true);
        aiColors = new List<string> { "red", "blue", "purple", "yellow", "green" };
        PauseV2.canPause = false;
        if (ManagePlayerHub.Instance)
        {
            ManagePlayerHub.Instance.GetComponent<PauseV2>().PauseMenuUI = pauseMenuUI;
            players = ManagePlayerHub.Instance.getPlayers();
            foreach (GameObject player in players)
            {
                aiColors.Remove(player.GetComponent<CharacterMovementController>().GetColorName());
                player.GetComponent<CharacterMovementController>().SetAimAssist(true);
            }
        } else
            Debug.LogWarning("Manage Player Hub not found!");
        PlayerInputManager.instance.DisableJoining();
        if (players != null)
        {
            for (int i = 0; i < players.Count; i++)
            {
                //Vector3 flagPos = GameObject.Find("Proto_Flag_01").transform.position;
                //players[i].transform.LookAt(new Vector3(transform.position.x, transform.position.y, transform.position.z));
                //players[i].transform.forward = new Vector3(0, 0, 1);
                players[i].transform.position = playerSpawns[i].position;
                players[i].transform.rotation = playerSpawns[i].rotation;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

        currentTime += Time.deltaTime;
        if (currentTime > waitTime && !spawnedPlayers)
        {
            Destroy(startCam);
            Destroy(startText);
            Destroy(startUI);
            //startText.text = "";
            if (players != null)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    if(players[i].GetComponent<CharacterMovementController>().isAI)
                    {
                        string aiColor = aiColors[0];
                        players[i].GetComponentInChildren<CharacterMovementController>(true).SetColorName(aiColor);
                        players[i].GetComponentInChildren<SkinnedMeshRenderer>(true).material = materials[GetColorIndex(aiColor)];
                        aiColors.Remove(aiColor);
                    }
                    spawnedPlayers = true;
                }
                ui.UpdateSpriteColors();
                DBGameManager.Instance.InitScores();
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
