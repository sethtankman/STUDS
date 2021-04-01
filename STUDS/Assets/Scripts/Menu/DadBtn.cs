using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadBtn : MonoBehaviour
{
    public Text btnText;
    public Image btnImage;
    public Sprite[] btnSprites; //This will be used once we have the eugine images.
    public GameObject player; //So the button can be connected to the player.
    public DadButtonMngr manager;

    private void Start()
    {
        manager = GameObject.Find("Canvas").GetComponent<DadButtonMngr>();
    }

    public void RandomPlayer()
    {
        ManagePlayerHub hub = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>();
        GameObject[] players = hub.getPlayers();
        int selected = (int)(Random.value * players.Length);
        int i = 0;
        foreach (GameObject _player in players) {
            if(selected == i)
            {
                player = _player;
            }
        }

        SetupPlayers();
    }


    /// <summary>
    /// Sets the sprite to the one specified.
    /// </summary>
    /// <param name="colorNum"></param>
    public void SetSprite(string colorName)
    {
        switch(colorName)
        {
            case "Blue":
                btnImage.sprite = btnSprites[0];
                break;
            case "Green":
                btnImage.sprite = btnSprites[1];
                break;
            case "Orange":
                btnImage.sprite = btnSprites[2];
                break;
            case "Purple":
                btnImage.sprite = btnSprites[3];
                break;
            case "Yellow":
                btnImage.sprite = btnSprites[4];
                break;
            default:
                Debug.LogError("Unable to set sprite to specified color: " + colorName);
                break;
        }
    }

    public void SetPlayer(GameObject refPlayer)
    {
        player = refPlayer;
        GetComponent<Button>().onClick.AddListener(SetupPlayers);
    }

    public void SetupPlayers()
    {
        manager.TransformPlayers(player);
    }
}
