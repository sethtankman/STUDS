using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DadButtonMngr : MonoBehaviour
{
    public List<GameObject> allPlayers, miniPlayers, dadPlayers;
    public GameObject[] allButtons;
    public GameObject characterButton, GameManager;
    public RectTransform[] buttonLocations;
    public string levelName;
    public int numAI, numPlayers;

    // Start is called before the first frame update
    void Start()
    {
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
            button.GetComponent<DadBtn>().SetPlayer(player);
            button.GetComponent<DadBtn>().manager = this;
            allButtons[i] = button;
            i++;
            numPlayers = i;
        }
    }

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
    }

    public void AddAI()
    {
        if (numPlayers + numAI < 4)
        {
            GameObject button = Instantiate(characterButton, buttonLocations[numPlayers+numAI].position, Quaternion.identity, gameObject.transform);
            button.GetComponent<DadBtn>().SetAI(true);
            button.GetComponent<RectTransform>().pivot = buttonLocations[numPlayers+numAI].pivot;
            allButtons[numPlayers + numAI] = button;
            numAI++;
        }
    }

    public void RemoveAI()
    {
        if (numAI > 0)
        {
            Destroy(allButtons[numPlayers + numAI - 1]);
            allButtons[numPlayers + numAI - 1] = null;
            numAI--;
        }
    }

    public void Accept()
    {
        foreach (GameObject player in miniPlayers)
        {
            player.GetComponent<CharacterMovementController>().SetToMini(true);
        }
        foreach (GameObject dad in dadPlayers)
        {
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
