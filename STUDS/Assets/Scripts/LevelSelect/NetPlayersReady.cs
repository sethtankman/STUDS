using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;


/// <summary>
/// Checks if players are ready to move into a level (Online only)
/// </summary>
public class NetPlayersReady : NetworkBehaviour
{
    public GameObject gameManager;
    private GameObject[] netPlayers;
    public List<GameObject> players;
    public AK.Wwise.Event EffectSound;
    public GameObject NetworkManager;
    private static KeyValuePair<string, int> PopularVote;
    private int Votes;
    // Start is called before the first frame update
    void Start()
    {
        PopularVote = new KeyValuePair<string, int>("None", 0);

        players = new List<GameObject>();
        NetworkManager = GameObject.Find("NetworkManager");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkCharacterMovementController>())
            {
                other.gameObject.GetComponent<NetworkCharacterMovementController>().ReadyPlayer(false, null);
                Votes--;
                if (PopularVote.Key == gameObject.tag)
                {
                    PopularVote = new KeyValuePair<string, int>(gameObject.tag, Votes);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer && other.CompareTag("Player"))
        {
            if (other.GetComponent<NetworkCharacterMovementController>())
            {
                bool allReady = true;
                other.GetComponent<NetworkCharacterMovementController>().ReadyPlayer(true, gameObject.tag);
                Votes++;
                if (Votes > PopularVote.Value)
                {
                    PopularVote = new KeyValuePair<string, int>(gameObject.tag, Votes);
                }
                netPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in netPlayers)
                {
                    if (player && !player.GetComponent<NetworkCharacterMovementController>().GetReadyPlayer())
                    {
                        EffectSound.Post(gameObject);
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    GameObject.Find("NetGameManager").GetComponent<NetGameManager>().RpcSaveState();
                    SteamLobby.singleton.SetLobbyClosed();
                    SteamMatchmaking.LeaveLobby(SteamLobby.singleton.joinedLobbyID);
                    NetPause.canPause = false;
                    NetPause.gameisPaused = false;
                    Invoke(nameof(ChangeScenes), 3.0f);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Votes > PopularVote.Value)
        {
            PopularVote = new KeyValuePair<string, int>(gameObject.tag, Votes);
        }
    }

    private void ChangeScenes()
    {
        switch (PopularVote.Key)
        {
            case "PennyPincher":
                StudsNetworkManager.singleton.ServerChangeScene("Net-PBDadRandom");
                break;
            case "StrollerRace":
                StudsNetworkManager.singleton.ServerChangeScene("NetNeighborhood");
                break;
            case "ShoppingSpree":
                StudsNetworkManager.singleton.ServerChangeScene("Network_Shopping_Spree");
                break;
            case "Downtown":
                StudsNetworkManager.singleton.ServerChangeScene("NetDowntown");
                break;
            case "Dodgeball":
                StudsNetworkManager.singleton.ServerChangeScene("NetDodgeball");
                break;
        }
    }
}
