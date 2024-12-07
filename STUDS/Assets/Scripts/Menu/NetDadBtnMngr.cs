using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetDadBtnMngr : NetworkBehaviour
{
    public List<GameObject> miniPlayers, dadPlayers;
    public List<string> remainingColors;
    public Stack<string> AIColors;
    public GameObject[] allButtons, allPlayers;
    public GameObject characterButton, GameManager, acceptButton;
    [SerializeField] private GameObject addAIBtn, subAIBtn;
    public RectTransform[] buttonLocations;
    public string levelName;
    public int numAI, numPlayers;

    // Start is called before the first frame update
    void Start()
    {
        PauseV2.canPause = false;
        acceptButton.SetActive(false);
        AIColors = new Stack<string>();
        remainingColors = new List<string>() { "blue", "red", "purple", "green", "yellow" };
        GameManager = GameObject.Find("NetGameManager");
        miniPlayers = new List<GameObject>();
        dadPlayers = new List<GameObject>();
        allButtons = new GameObject[4];
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        if (isServer)
            Invoke(nameof(InitBtns), 1.0f);
    }

    public void InitBtns()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        int i = 0;
        acceptButton.SetActive(true);
        addAIBtn.SetActive(true);
        subAIBtn.SetActive(true);
        foreach (GameObject player in allPlayers)
        {
            GameObject button = Instantiate(characterButton, buttonLocations[i].position, Quaternion.identity, gameObject.transform);
            NetworkServer.Spawn(button);
            button.GetComponent<RectTransform>().pivot = buttonLocations[i].pivot;
            button.GetComponent<NetDadBtn>().SetSprite(player.GetComponent<NetworkCharacterMovementController>().GetColorName(), false);
            remainingColors.Remove(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
            button.GetComponent<NetDadBtn>().SetPlayer(player);
            button.GetComponent<NetDadBtn>().manager = this;
            allButtons[i] = button;

            if (i > 0) // This sort of performs it backwards: add the player to the wrong list, then switch them.
            {
                button.GetComponent<NetDadBtn>().ToggleMini();
            }
            else
            { // first player is a dad.
                miniPlayers.Add(player);
                ToggleMini(player);
            }
            i++;
            numPlayers = i;
        }
        Invoke(nameof(ClientInitialize), 1.0f);
    }

    public void ClientInitialize()
    {
            foreach (NetDadBtn button in GetComponentsInChildren<NetDadBtn>())
            {
                button.RpcSetPivot(button.GetComponent<RectTransform>().pivot);
                button.RpcSetSprite(button.color, button.imageIndex > 4);
                //button.GetComponent<NetDadBtn>().RpcSetPlayer(button.player);
            }
    }

    /*[ClientRpc] // this works, but the dad information isn't consistent across the network
    public void RpcClientInitialize()
    {
        if (!isServer && needsUpdate)
        {
            int i = 0;
            foreach (NetDadBtn button in GetComponentsInChildren<NetDadBtn>())
            {
                if (i < allPlayers.Count )
                {
                    GameObject player = allPlayers[i];
                    button.GetComponent<RectTransform>().pivot = buttonLocations[i].pivot;
                    button.GetComponent<NetDadBtn>().SetSprite(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
                    remainingColors.Remove(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
                    button.GetComponent<NetDadBtn>().SetPlayer(player);
                    button.GetComponent<NetDadBtn>().manager = this;
                    allButtons[i] = button.gameObject;

                    i++;
                    needsUpdate = false;
                }
            }
        }
    }*/

    public void ToggleMini(GameObject selectedPlayer)
    {
        if (miniPlayers.Contains(selectedPlayer))
        {
            miniPlayers.Remove(selectedPlayer);
            dadPlayers.Add(selectedPlayer);
        }
        else
        {
            miniPlayers.Add(selectedPlayer);
            dadPlayers.Remove(selectedPlayer);
        }
        // The number of dads cannot exceed the number of kids by more than one.
        if (isServer)
        {
            if (dadPlayers.Count > miniPlayers.Count + numAI + 1 || dadPlayers.Count == 0)
            {
                acceptButton.SetActive(false);
            }
            else
            {
                acceptButton.SetActive(true);
            }
        }
    }

    public void AddAI()
    {
        if (isServer && numPlayers + numAI < 4)
        {
            GameObject button = Instantiate(characterButton, buttonLocations[numPlayers + numAI].position, Quaternion.identity, transform);
            NetworkServer.Spawn(button);
            button.GetComponent<NetDadBtn>().RpcSetSprite(remainingColors.ToArray()[0], false);
            button.GetComponent<NetDadBtn>().color = remainingColors.ToArray()[0];
            AIColors.Push(remainingColors.ToArray()[0]);
            remainingColors.Remove(remainingColors.ToArray()[0]);
            button.GetComponent<NetDadBtn>().RpcSetAI(true);
            button.GetComponent<NetDadBtn>().RpcSetPivot(buttonLocations[numPlayers + numAI].pivot);
            allButtons[numPlayers + numAI] = button;
            numAI++;

            if (dadPlayers.Count > miniPlayers.Count + numAI + 1 || dadPlayers.Count == 0)
            {
                acceptButton.SetActive(false);
            }
            else
            {
                acceptButton.SetActive(true);
            }
        }
    }

    public void RemoveAI()
    {
        if (isServer && numAI > 0)
        {
            remainingColors.Add(allButtons[numPlayers + numAI - 1].GetComponent<NetDadBtn>().color);
            AIColors.Pop();
            NetworkServer.Destroy(allButtons[numPlayers + numAI - 1]);
            allButtons[numPlayers + numAI - 1] = null;
            numAI--;

            if (dadPlayers.Count > miniPlayers.Count + numAI + 1 || dadPlayers.Count == 0)
            {
                acceptButton.SetActive(false);
            }
            else
            {
                acceptButton.SetActive(true);
            }
        }
    }

    public void Accept()
    {
        foreach (GameObject player in miniPlayers)
        {
            Debug.Log("Mini: " + player);
            player.GetComponent<NetworkCharacterMovementController>().RpcSetToMini(true);
        }
        foreach (GameObject dad in dadPlayers)
        {
            Debug.Log("Dad: " + dad);
            dad.GetComponent<NetworkCharacterMovementController>().RpcSetToMini(false);
        }

        Invoke("LoadLevel", 1.0f); // Calling this after a delay so NetworkTransform has a chance to update scale before changing scenes.
    }

    private void LoadLevel()
    {
        NetGameManager mngr = GameManager.GetComponent<NetGameManager>();
        mngr.numAIToSpawnPB = numAI;
        mngr.aiColors = AIColors;
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().ServerChangeScene(levelName);
    }
}
