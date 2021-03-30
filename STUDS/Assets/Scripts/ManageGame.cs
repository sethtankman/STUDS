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

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Searching for : " + soundName);
        GameObject sfx = GameObject.Find("SFX");
        Transform trans = sfx.transform;
        Transform target = trans.Find(soundName);
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
        Debug.Log("Collided with end");
        if (collider.tag.Equals("Player"))
        {
            Debug.Log("Player checkpoint: " + collider.gameObject.GetComponent<CharacterMovementController>().getCheckpointCount());
            if(collider.gameObject.GetComponent<CharacterMovementController>().getCheckpointCount() == checkpoints.Length && collider.gameObject.GetComponent<CharacterMovementController>().GetHasGrabbed())
            {
                if(collider.gameObject.GetComponent<CharacterMovementController>().GetFinishPosition() == -1)
                {
                    Debug.Log("Finished!");
                    display = true;
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
