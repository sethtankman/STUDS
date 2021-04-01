using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class DadButtonMngr : MonoBehaviour
{
    private GameObject[] players;
    private GameObject canv;
    public GameObject characterButton;
    public Transform[] buttonLocations;
    public string levelName;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputManager.instance.DisableJoining();  //Technically we shouldn't need this, because we should disable joining before level select.
        players = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>().players;
        //if there is only one player, load into the next level.
        if (players.Length == 1)
        {
            LoadLevel();
        }
        else
        {
            canv = GameObject.Find("Canvas");
            //if there are multiple players, spawn buttons
            int i = 0;
            foreach (GameObject player in players)
            {
                GameObject button = Instantiate(characterButton, buttonLocations[i].position, Quaternion.identity, canv.transform);
                button.GetComponent<DadBtn>().SetSprite(player.GetComponent<CharacterMovementController>().GetColorName());
                button.GetComponent<DadBtn>().SetPlayer(player);
                i++;
            }
        }
    }

    public void TransformPlayers(GameObject dad)
    {
        foreach (GameObject player in players)
        {
            TransformDadToKid(player);
        }
        TransformKidToDad(dad);
        LoadLevel();
    }

    private void TransformDadToKid(GameObject player)
    {
        player.transform.localScale = new Vector3(20, 20, 20); //Shrink the player. OG size is 30, 30, 30
        player.GetComponent<CharacterMovementController>().SetBinky(true); //Activate the binky!!!!
        player.GetComponent<CharacterMovementController>().isMini = true; //The Eugine will now act as a child.
    }

    private void TransformKidToDad(GameObject player)
    {
        player.transform.localScale = new Vector3(30, 30, 30); //Shrink the player. OG size is 30, 30, 30
        player.GetComponent<CharacterMovementController>().SetBinky(false); //Activate the binky!!!!
        player.GetComponent<CharacterMovementController>().isMini = false; //The Eugine will now act as a child.
    }

    private void LoadLevel()
    {
        SceneManager.LoadScene(levelName);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
