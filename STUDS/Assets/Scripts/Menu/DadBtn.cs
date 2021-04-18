using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadBtn : MonoBehaviour
{
    public Text btnText;
    public Image btnImage;
    public Sprite[] btnSprites; //This will be used once we have the eugine images.
    public GameObject player, miniImage, aiImage; //So the button can be connected to the player.
    public DadButtonMngr manager;

    private void Start()
    {
        manager = GameObject.Find("Panel").GetComponent<DadButtonMngr>();
    }

    public void RandomPlayer()
    {
        List<GameObject> players = new List<GameObject>();
        players = ManagePlayerHub.Instance.getPlayers();
        int selected = (int)(Random.value * players.Count);
        int i = 0;
        foreach (GameObject _player in players)
        {
            if (selected == i)
            {
                player = _player;
            }
        }
    }


    /// <summary>
    /// Sets the sprite to the one specified.
    /// </summary>
    /// <param name="colorNum"></param>
    public void SetSprite(string colorName)
    {
        switch (colorName)
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
        GetComponent<Button>().onClick.AddListener(ToggleMini);
    }

    public void ToggleMini()
    {
        if (aiImage.activeSelf == false)
        {
            // If we are to change a kid to a dad, the number of dads cannot exceed the number of kids by more than one.
            if(miniImage.activeSelf && manager.dadPlayers.Count <= manager.numAI + manager.miniPlayers.Count)
            {
                Debug.Log("Number of dads cannot exceed the number of kids by more than one");
                return;
            }
            miniImage.SetActive(!miniImage.activeSelf);
            manager.ToggleMini(player);
        } else
        {
            miniImage.SetActive(!miniImage.activeSelf);
            manager.ToggleMini(player);
        }
    }

    public void SetAI(bool setAI)
    {
        aiImage.SetActive(setAI);
    }
}
