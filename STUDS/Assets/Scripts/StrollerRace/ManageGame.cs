using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class ManageGame : MonoBehaviour
{

    private List<PlayerConfiguration> configs;

    public GameObject[] checkpoints;

    public TextMeshProUGUI FinishText;
    public TextMeshProUGUI FinishTimer;

    public SteamAchievements sa;

    public float endTimer;

    public float swapTime;

    public string soundName;

    public AK.Wwise.Event mySource;

    private float timeCount;
    private bool display;
    private int playerID;
    private float timer;
    private bool playerFinish = false;

    private int positions = 1;
    private int noFinishPositions = -1;

    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            Debug.Log("Text active");
            timeCount += Time.deltaTime;
            FinishText.text = "Player " + playerID + " has finished the race!";
            playerFinish = true;
            if (timeCount >= swapTime)
            {
                display = false;
                timeCount = 0;
            }
        }
        else if (!display)
        {
            FinishText.text = "";
        }

        if (playerFinish)
        {
            timer += Time.deltaTime;
            FinishTimer.text = "A player has finished the race! The race will end in " + (int)(endTimer - timer) + " seconds!";
            if(timer >= endTimer)
            {
                GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

                foreach (GameObject player in allPlayers)
                {
                    if (player.GetComponent<CharacterMovementController>().isAI)
                    {
                        DontDestroyOnLoad(player.transform.parent.gameObject);
                        ManagePlayerHub.Instance.AddPlayer(player);
                        Debug.Log("New AI added!");
                    }
                }
                List<GameObject> players = ManagePlayerHub.Instance.getPlayers();
                foreach (GameObject player in players)
                {
                    if(player.GetComponentInChildren<CharacterMovementController>().GetFinishPosition() == 0)
                    {
                        player.GetComponentInChildren<CharacterMovementController>().SetFinishPosition(noFinishPositions);
                        noFinishPositions--;
                    }
                }
                SceneManager.LoadScene("VictoryStands");
            }
        }
        else
        {
            FinishTimer.text = "";
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag.Equals("Player"))
        {
            if(collider.gameObject.GetComponent<CharacterMovementController>().getCheckpointCount() == checkpoints.Length && collider.gameObject.GetComponent<CharacterMovementController>().GetHasGrabbed())
            {
                if(collider.gameObject.GetComponent<CharacterMovementController>().GetFinishPosition() == 0)
                {
                    display = true;
                    if (SceneManager.GetActiveScene().name.Equals("TheBlock_Scott"))
                        sa.UnlockAchievement("SR_COMPLETE");
                    else
                        sa.UnlockAchievement("SR_DOWNTOWN");
                    playerID = collider.gameObject.GetComponent<CharacterMovementController>().getPlayerID() + 1;
                    collider.gameObject.GetComponent<CharacterMovementController>().SetFinishPosition(positions);
                    positions++;
                    if (collider.gameObject.GetComponent<CharacterMovementController>().isAI == false)
                    {
                        mySource.Post(gameObject);
                    }
                }

            }
        }
    }

    public void updatePlayerConfigsList(List<PlayerConfiguration> configs)
    {
        foreach(PlayerConfiguration config in configs)
        {
            if (!this.configs.Contains(config))
            {
                this.configs.Add(config);
            }
        }
    }
}
