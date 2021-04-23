using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DadButtonMngr : MonoBehaviour
{
    public List<GameObject> allPlayers, miniPlayers, dadPlayers;
    public List<string> remainingColors;
    public GameObject[] allButtons;
    public GameObject characterButton, GameManager, acceptButton;
    public RectTransform[] buttonLocations;
    public string levelName;
    public int numAI, numPlayers;

    // Start is called before the first frame update
    void Start()
    {
        remainingColors = new List<string>() { "Blue", "Red", "Purple", "Green", "Yellow"};
        GameManager = GameObject.Find("GameManager");
        miniPlayers = new List<GameObject>();
        dadPlayers = new List<GameObject>();
        PlayerInputManager.instance.DisableJoining();  //Technically we shouldn't need this, because we should disable joining before level select.
        allPlayers = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>().players;
        allButtons = new GameObject[4];
        int i = 0;
        foreach (GameObject player in allPlayers)
        {
            GameObject button = Instantiate(characterButton, buttonLocations[i].position, Quaternion.identity, gameObject.transform);
            button.GetComponent<RectTransform>().pivot = buttonLocations[i].pivot;
            button.GetComponent<DadBtn>().SetSprite(player.GetComponent<CharacterMovementController>().GetColorName());
            remainingColors.Remove(player.GetComponent<CharacterMovementController>().GetColorName());
            button.GetComponent<DadBtn>().SetPlayer(player);
            button.GetComponent<DadBtn>().manager = this;
            allButtons[i] = button;
            Debug.Log("Button added");
            if(i > 0) // This sort of performs it backwards: add the player to the wrong list, then switch them.
            {
                button.GetComponent<DadBtn>().miniImage.SetActive(true);
                ToggleMini(player);
            } else
            {
                Debug.Log("Adding: " + player);
                miniPlayers.Add(player);
                ToggleMini(player);
            }
            i++;
            numPlayers = i;
        }
    }

    public void ToggleMini(GameObject selectedPlayer)
    {
        if (miniPlayers.Contains(selectedPlayer))
        {
            Debug.Log("Removing: " + selectedPlayer);
            miniPlayers.Remove(selectedPlayer);
            dadPlayers.Add(selectedPlayer);
        }
        else
        {
            Debug.Log("Adding1: " + selectedPlayer);
            miniPlayers.Add(selectedPlayer);
            dadPlayers.Remove(selectedPlayer);
        }
        // The number of dads cannot exceed the number of kids by more than one.
        if (dadPlayers.Count > miniPlayers.Count + numAI + 1 || dadPlayers.Count == 0)
        {
            acceptButton.SetActive(false);
        } else
        {
            acceptButton.SetActive(true);
        }
    }

    public void AddAI()
    {
        if (numPlayers + numAI < 4)
        {
            GameObject button = Instantiate(characterButton, buttonLocations[numPlayers+numAI].position, Quaternion.identity, gameObject.transform);
            button.GetComponent<DadBtn>().SetSprite(remainingColors.ToArray()[0]);
            button.GetComponent<DadBtn>().color = remainingColors.ToArray()[0];
            remainingColors.Remove(remainingColors.ToArray()[0]);
            button.GetComponent<DadBtn>().SetAI(true);
            button.GetComponent<RectTransform>().pivot = buttonLocations[numPlayers+numAI].pivot;
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
        if (numAI > 0)
        {
            remainingColors.Add(allButtons[numPlayers + numAI - 1].GetComponent<DadBtn>().color);
            Destroy(allButtons[numPlayers + numAI - 1]);
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
            player.GetComponent<CharacterMovementController>().SetToMini(true);
        }
        foreach (GameObject dad in dadPlayers)
        {
            Debug.Log("Dad: " + dad);
            dad.GetComponent<CharacterMovementController>().SetToMini(false);
        }
        LoadLevel();
    }

    private void LoadLevel()
    {
        GameManager.GetComponent<ManagePlayerHub>().numAIToSpawnPB = numAI;
        SceneManager.LoadScene(levelName);
    }

    // Update is called once per frame
    void Update()
    {


    }
}
