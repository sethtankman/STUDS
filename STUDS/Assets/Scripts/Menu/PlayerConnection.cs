using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerConnection : MonoBehaviour
{
    public ManagePlayerHub playerHub;

    private Dictionary<string, int> colorImages;
    public Sprite[] eugeneVarieties;
    public Image[] playerImages;
    public GameObject[] textObjects;

    private int clickedPlayer;

    public void Start()
    {
        colorImages = new Dictionary<string, int>();
        colorImages["Blue"] = 0;
        colorImages["Purple"] = 1;
        colorImages["Red"] = 2;
        colorImages["Yellow"] = 3;
        colorImages["Green"] = 4;
        playerHub = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>();
        int i = 0;
        foreach (GameObject player in playerHub.players)
        {
            if (player)
            {
                SetPanelImage(i, player.GetComponent<CharacterMovementController>().GetColorName());
            }
            i++;
        }
    }

    public void SetPanelImage(int panelID, string colorName)
    {
        colorImages = new Dictionary<string, int>();
        colorImages["Blue"] = 0;
        colorImages["Purple"] = 1;
        colorImages["Red"] = 2;
        colorImages["Yellow"] = 3;
        colorImages["Green"] = 4;
        playerImages[panelID].color = Color.white;
        Debug.Log("This is my color: " + colorName);
        playerImages[panelID].sprite = eugeneVarieties[colorImages[colorName]];
        textObjects[panelID].GetComponent<TextMeshProUGUI>().text = "PLAYER " + (panelID+1);
    }

    public void KickPlayer()
    {
        for(int i = clickedPlayer; i < 3; i++)
        {
            Debug.Log("Control reached: " + i);
            if(i+1 == playerHub.playerIDCount)
            {
                playerImages[i].color = Color.gray;
                textObjects[i].GetComponent<TextMeshProUGUI>().text = "NO CONTROLLER DETECTED";
                playerHub.KickPlayer(clickedPlayer);
                return;
            } else
            {
                playerImages[i].sprite = playerImages[i + 1].sprite;
            }
        }
    }

    public void SetClickedPlayer(int id)
    {
        clickedPlayer = id;
    }
}
