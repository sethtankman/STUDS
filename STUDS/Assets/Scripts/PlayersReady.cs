using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayersReady : MonoBehaviour
{
    public GameObject gameManager;
    List<GameObject> players;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.GetComponent<CharacterMovementController>().ReadyPlayer(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        bool allReady = true;
        other.gameObject.GetComponent<CharacterMovementController>().ReadyPlayer(true);
        players = gameManager.GetComponent<ManagePlayerHub>().getPlayers();
        foreach(GameObject player in players)
        {
            if (!player.GetComponent<CharacterMovementController>().GetReadyPlayer())
            {
                allReady = false;
            }
        }
        if (allReady)
        {
            gameManager.GetComponent<ManagePlayerHub>().SaveState();
            if (gameObject.tag.Equals("PennyPincher"))
            {
                SceneManager.LoadScene("PBDadRandomizer");
            }
            else if (gameObject.tag.Equals("StrollerRace"))
            {
                SceneManager.LoadScene("TheBlock_Scott");
            }
            else if (gameObject.tag.Equals("ShoppingSpree"))
            {
                SceneManager.LoadScene("Shopping_Spree-Scott");
            }

        }

    }
}
