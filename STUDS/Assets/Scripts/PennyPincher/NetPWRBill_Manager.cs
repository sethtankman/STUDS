using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class NetPWRBill_Manager : NetworkBehaviour
{
    //Electricity variables
    public float Score;
    public TextMeshProUGUI PowerTXT;
    public int NumItemsOn;
    public TextMeshProUGUI ItemsOnTXT;

    //List of objects to interact with
    public List<NetInteraction> Interactives = new List<NetInteraction>();
    public List<int> Validation = new List<int>();
    public int MaxObjectsOff;

    //Timer for the end of the game
    public TextMeshProUGUI TimerTXT;
    public float timer;
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
        NumItemsOn = Interactives.Count;

        if (isServer)
        {
            for (int j = 0; j < MaxObjectsOff; j++)
            {
                ValidatePicks();
            }

            //Invoke("SetInteractives", 4.0f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.RoundToInt(timer);

        PowerTXT.text = "Power Bill: $" + (Score / 10) + "0";
        ItemsOnTXT.text = "Appliances: " + (NumItemsOn + 1);
               
        timer -= Time.deltaTime;

        if (timer > 0.0f)
        {
            Showtime(timer);
        }
        else
        {
            if(!ended)
                EndGame();
        }

    }

    [ClientRpc]
    public void RpcSetInteractives(List<int> _validation)
    {
        Validation = _validation;
        Debug.Log(Validation.Count);
        for (int i = 0; i < MaxObjectsOff; i++)
        {
            Debug.Log(Validation[i]);
            Debug.Log(Interactives[Validation[i]]);
            Interactives[Validation[i]].ToggleVisualGM();
        }
    }

    private void SetInteractives()
    {
        RpcSetInteractives(Validation);
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

        GameObject[] allPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in allPlayers)
        {
            if (player.GetComponent<NetworkCharacterMovementController>().isAI)
            {
                player.GetComponent<NetPennyPincherAI>().SetActive(false);
                DontDestroyOnLoad(player.gameObject);
                //NetGameManager.Instance.AddPlayer(player);
                //Debug.Log("New AI added!");
            }
        }
        if (isServer)
        {
            List<GameObject> players = NetGameManager.Instance.getPlayers();
            foreach (GameObject player in players) // TODO: Doesn't account for which side won!
            {
                if (player.GetComponent<NetworkCharacterMovementController>())
                {
                    var controller = player.GetComponent<NetworkCharacterMovementController>();
                    if (controller.finishPosition == 0)
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
                    var controller = player.GetComponentInChildren<NetworkCharacterMovementController>();
                    if (controller.finishPosition == 0)
                    {
                        if (controller.isMini)
                        {
                            controller.SetFinishPosition(noFinishPos);
                            noFinishPos--;
                            if (player.GetComponentInChildren<KidTimeout>())
                                player.GetComponentInChildren<KidTimeout>().enabled = false;
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
        }

        netManager.ServerChangeScene("NetVictoryStands");
    }

    void Showtime(float timeleft)
    {
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
        int picked = Random.Range(0, Interactives.Count);
        while (Validation.Contains(picked))
        {
            picked = Random.Range(0, Interactives.Count);
        }
        Validation.Add(picked);

    }
}
