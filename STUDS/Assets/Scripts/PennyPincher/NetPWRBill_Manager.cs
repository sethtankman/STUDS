using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;
using System;

public class NetPWRBill_Manager : NetworkBehaviour
{
    public static GameObject[] timeoutPos;
    public static GameObject[] backInPos;

    //Electricity variables
    [SyncVar]
    public float Score;
    public TextMeshProUGUI PowerTXT;
    [SyncVar]
    public int NumItemsOn;
    public TextMeshProUGUI ItemsOnTXT;

    //List of objects to interact with
    public List<NetInteraction> Interactives = new List<NetInteraction>();
    public List<int> Validation = new List<int>();
    public int MaxObjectsOff;

    //Timer for the end of the game
    public TextMeshProUGUI TimerTXT;
    private float serverStartTime;
    [SerializeField] private float gameTime;
    private float Sprint = 10.0f;


    public GameObject PBGameEndText;
    private StudsNetworkManager netManager;

    private int racePositions;
    private int noFinishPos;
    private bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        racePositions = 1;
        noFinishPos = -1;
        netManager = GameObject.Find("NetworkManager").GetComponent<StudsNetworkManager>();
        foreach (GameObject Electronic in GameObject.FindGameObjectsWithTag("RandomPick"))
        {
            Interactives.Add(Electronic.GetComponent<NetInteraction>());
            if (Interactives.Contains(null))
            {
                Interactives.Remove(null);
            }

        }
        if (isServer)
        {
            NumItemsOn = Interactives.Count;

            for (int j = 0; j < MaxObjectsOff; j++)
            {
                ValidatePicks();
            }

            Invoke("SetInteractives", 3.0f);
        }
        serverStartTime = (float)NetworkTime.time;
    }

    // Update is called once per frame
    void Update()
    {

        PowerTXT.text = "Power Bill: $" + (Score / 10) + "0";
        ItemsOnTXT.text = "Appliances: " + (NumItemsOn + 1);

        if ((float)NetworkTime.time  < serverStartTime + gameTime)
        {
            Showtime();
        }
        else if(!ended) { 
            EndGame();
        }

    }

    private void SetInteractives()
    {
        // RpcSetInteractives(Validation);
        for (int i = 0; i < MaxObjectsOff; i++)
        {
            Interactives[Validation[i]].RpcToggleVisualGM();
        }
    }

    public void AddScore(int toAdd)
    {
        Score += toAdd;
    }

    void EndGame()
    {
        ended = true;
        GameObject EndText = Instantiate(PBGameEndText);
        DontDestroyOnLoad(EndText);
        EndText.GetComponent<TextMeshProUGUI>().text = Score.ToString();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<NetKidTimeout>().inPowerBill = false;
            if (player.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                player.GetComponent<NetPennyPincherAI>().SetActive(false);
                DontDestroyOnLoad(player);
            }
        }
        if (isServer)
        {
            foreach (GameObject player in players) // TODO: Doesn't account for which side won!
            {
                var controller = player.GetComponent<NetworkCharacterMovementController>();
                if (controller)
                {
                    if (controller.GetFinishPosition() == 0)
                    {
                        if (controller.isMini)
                        {
                            controller.SetFinishPosition(noFinishPos);
                            noFinishPos--;
                        }
                        else
                        {
                            controller.SetFinishPosition(racePositions);
                            racePositions++;
                        }
                    }
                }
                else if (player.GetComponentInChildren<NetworkCharacterMovementController>()) {
                    controller = player.GetComponentInChildren<NetworkCharacterMovementController>();
                    if (controller.GetFinishPosition() == 0)
                    {
                        if (controller.isMini)
                        {
                            controller.SetFinishPosition(noFinishPos);
                            noFinishPos--;
                            if (player.GetComponentInChildren<NetKidTimeout>())
                                player.GetComponentInChildren<NetKidTimeout>().enabled = false;
                        }
                        else
                        {
                            controller.SetFinishPosition(racePositions);
                            racePositions++;
                        }
                    }
                } else
                {
                    Debug.LogError("No Network Character movement controller found in player!");
                }
            }
            netManager.ServerChangeScene("NetVictoryStands");
        }
    }

    void Showtime()
    {
        float timeleft = gameTime - (float)NetworkTime.time + serverStartTime;
        float min = Mathf.FloorToInt(timeleft / 60);
        float sec = Mathf.FloorToInt(timeleft % 60);

        if(sec == Sprint && min == 0.0f)
        {
            TimerTXT.fontSize += 10;
            Sprint -= 1.0f;
        }

        TimerTXT.text = string.Format("{0:00}:{1:00}", min, sec);       

    }

    /// <summary>
    /// Fills global variable Validation with distinct random integers used for determining which objects to be on or off
    /// </summary>
    void ValidatePicks()
    {
        int picked = UnityEngine.Random.Range(0, Interactives.Count);
        while (Validation.Contains(picked))
        {
            picked = UnityEngine.Random.Range(0, Interactives.Count);
        }
        Validation.Add(picked);

    }
}
