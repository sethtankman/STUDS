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
            if (other.gameObject.GetComponent<NetworkCharacterMovementController>())
            {
                other.gameObject.GetComponent<NetworkCharacterMovementController>().ReadyPlayer(false, null);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isServer && other.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<NetworkCharacterMovementController>())
            {
                bool allReady = true;
                other.gameObject.GetComponent<NetworkCharacterMovementController>().ReadyPlayer(true, gameObject.tag);

                netPlayers = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject player in netPlayers)
                {
                    if (player && !player.GetComponent<NetworkCharacterMovementController>().GetReadyPlayer(gameObject.tag))
                    {
                        EffectSound.Post(gameObject);
                        allReady = false;
                    }
                }
                if (allReady)
                {
                    GameObject.Find("NetGameManager").GetComponent<NetGameManager>().RpcSaveState();
                    StudsNetworkManager netManager = NetworkManager.GetComponent<StudsNetworkManager>();
                    // Close the lobby
                    SteamLobby.singleton.SetLobbyClosed();
                    SteamMatchmaking.LeaveLobby(SteamLobby.singleton.joinedLobbyID);
                    if (gameObject.CompareTag("PennyPincher"))
                    {
                        netManager.ServerChangeScene("Net-PBDadRandom");
                    }
                    else if (gameObject.CompareTag("StrollerRace"))
                    {
                        netManager.ServerChangeScene("NetBlock");
                    }
                    else if (gameObject.CompareTag("ShoppingSpree"))
                    {
                        netManager.ServerChangeScene("Network_Shopping_Spree");
                    }
                    else if (gameObject.CompareTag("Downtown"))
                    {
                        netManager.ServerChangeScene("NetDowntown");
                    } else if (gameObject.CompareTag("Dodgeball"))
                    {
                        netManager.ServerChangeScene("NetDodgeball");
                    }
                }
            }
        }
    }
}
