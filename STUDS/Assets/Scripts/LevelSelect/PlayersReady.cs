using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Checks if players are ready to move into a level (Offline only)
/// </summary>
public class PlayersReady : MonoBehaviour
{
    public GameObject gameManager;
    private GameObject[] netPlayers;
    public List<GameObject> players;
    public AK.Wwise.Event EffectSound;
    public GameObject NetworkManager;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        NetworkManager = GameObject.Find("NetworkManager");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<CharacterMovementController>())
            {
                other.gameObject.GetComponent<CharacterMovementController>().ReadyPlayer(false, null);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<CharacterMovementController>())
            {
                bool allReady = true;
                other.gameObject.GetComponent<CharacterMovementController>().ReadyPlayer(true, gameObject.tag);

                players = gameManager.GetComponent<ManagePlayerHub>().getPlayers();
                foreach (GameObject player in players)
                {
                    if (player && !player.GetComponent<CharacterMovementController>().GetReadyPlayer(gameObject.tag))
                    {
                        EffectSound.Post(gameObject);
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    gameManager.GetComponent<ManagePlayerHub>().SaveState();
                    if (gameObject.CompareTag("PennyPincher"))
                    {
                        SceneManager.LoadScene("PBDadRandomizer");
                    }
                    else if (gameObject.CompareTag("StrollerRace"))
                    {
                        SceneManager.LoadScene("TheBlock_Scott");
                    }
                    else if (gameObject.CompareTag("ShoppingSpree"))
                    {
                        SceneManager.LoadScene("Shopping_Spree-Scott");
                    }
                    else if (gameObject.CompareTag("Downtown"))
                    {
                        SceneManager.LoadScene("Downtown_prototype");
                    }
                    else if (gameObject.CompareTag("Dodgeball"))
                    {
                        SceneManager.LoadScene("Dodgeball_v2");
                    }

                }
            }
        }
    }
}
