using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NetCheckpoint : MonoBehaviour
{
    public int checkPointNum;

    public TextMeshProUGUI checkPointText;

    public float swapTime;

    public string soundName;

    public AK.Wwise.Event mySource;
    private float timeCount;
    private bool display;
    private int playerID;

    // Start is called before the first frame update
    void Start()
    {
        display = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (display)
        {
            timeCount += Time.deltaTime;
            checkPointText.text = "Player " + playerID + " has passed checkpoint " + checkPointNum + "!";
            if(timeCount >= swapTime)
            {
                display = false;
                timeCount = 0;
                checkPointText.text = "";
            }
        }
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            NetworkCharacterMovementController player = collider.GetComponent<NetworkCharacterMovementController>();
            if(player)
            { // players will always have NetworkCharacterMovementController in Network mode
                GameObject stroller = player.GetGrabbedObject();
                if ((player.getCheckpointCount() == checkPointNum - 1) && stroller != null)
                {
                    if ((stroller.GetComponent<StrollerController>().StrollerID == player.getPlayerID()))
                    {
                        player.SetCheckpointCount(checkPointNum);
                        display = true;
                        playerID = player.getPlayerID() + 1;
                        mySource.Post(gameObject);
                    }

                }
            } else
            { // AI will not have Network CMC
                CharacterMovementController AI = collider.GetComponent<CharacterMovementController>();
                GameObject stroller = AI.GetGrabbedObject();
                if ((AI.getCheckpointCount() == checkPointNum - 1) && stroller != null)
                {
                    AI.SetCheckpointCount(checkPointNum);
                    display = true;
                    playerID = AI.getPlayerID() + 1;
                }
            }
            

        }
    }
}
