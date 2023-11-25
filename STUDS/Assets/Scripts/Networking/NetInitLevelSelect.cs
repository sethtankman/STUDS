using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetInitLevelSelect : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;

    // Start is called before the first frame update
    void Start()
    {
        // We only need this when we revisit the levelselect.
        if (NetGameManager.Instance)
        {
            NetGameManager.Instance.GetComponent<PauseV2>().PauseMenuUI = PausePanel;
            List<GameObject> players = NetGameManager.Instance.getPlayers();
            bool[] AI_ids = new bool[4];
            int i = 0;
            while (i < players.Count)
            {
                GameObject player = players[i];
                // Move players into the right place
                if (player.GetComponent<NetworkCharacterMovementController>().isAI)
                {
                    Destroy(player.transform.parent.gameObject);
                    AI_ids[i] = true;
                } else {
                    player.transform.position = new Vector3(-5, 0, 32);
                    player.SetActive(true);
                    player.GetComponentInChildren<StrollerLocator>().SetActive(false);
                }
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

            foreach (GameObject player in NetGameManager.Instance.players)
            {
                player.GetComponent<NetworkCharacterMovementController>().SetAimAssist(true);
                player.GetComponent<NetworkCharacterMovementController>().finishPosition = 0;
            }
        }
    }
}
