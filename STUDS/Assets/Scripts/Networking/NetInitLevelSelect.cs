using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetInitLevelSelect : NetworkBehaviour
{
    public GameObject PausePanel;

    // Start is called before the first frame update
    void Start()
    {
        NetPause.canPause = true;
        // We only need this when we revisit the levelselect.
        if (NetGameManager.Instance)
        {
            NetGameManager.Instance.SetPauseMenuPanel(PausePanel);
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            bool[] AI_ids = new bool[4];
            int i = 0;
            while (i < players.Length)
            {
                GameObject player = players[i];
                // Move players into the right place
                if (player.GetComponent<NetworkCharacterMovementController>().isAI)
                {
                    if (transform.parent)
                        Destroy(player.transform.parent.gameObject);
                    else
                        Destroy(player);
                    AI_ids[i] = true;
                } 
                player.GetComponent<NetworkCharacterMovementController>().inStrollerRace = false;
                player.GetComponent<NetworkCharacterMovementController>().CanMove = true;
                ++i;
            }
            i = 0;
            for(int j = 0; j < 4; j++)
            {
                if (AI_ids[j])
                    NetGameManager.Instance.RemovePlayerAssignments(i);
                else
                    ++i;
            }

            foreach (GameObject player in NetGameManager.Instance.AIplayers)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
                player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(0);
            }
        }
        else {
            FindObjectOfType<NetGameManager>().SetPauseMenuPanel(PausePanel);
        }
    }
}
