using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadBtn : MonoBehaviour
{
    public Text btnText;
    public Image btnImage;
    public Sprite[] btnSprites; //This will be used once we have the eugine images.
    public Material[] kidMaterials;
    public GameObject player, aiImage; //So the button can be connected to the player.
    public DadButtonMngr manager;
    public string color;
    private int imageIndex;

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
                imageIndex = 0;
                break;
            case "Green":
                btnImage.sprite = btnSprites[1];
                imageIndex = 1;
                break;
            case "Red":
                btnImage.sprite = btnSprites[2];
                imageIndex = 2;
                break;
            case "Purple":
                btnImage.sprite = btnSprites[3];
                imageIndex = 3;
                break;
            case "Yellow":
                btnImage.sprite = btnSprites[4];
                imageIndex = 4;
                break;
            default:
                Debug.LogError("Unable to set sprite to specified color: " + colorName);
                break;
        }
    }

    public void SetPlayer(GameObject refPlayer)
    {
        player = refPlayer;
        // GetComponent<Button>().onClick.AddListener(ToggleMini);
    }

    public void ToggleMini()
    {
        if (aiImage.activeSelf == false)
        {
            
            if(imageIndex > 4)
            {
                Debug.Log("Setting to dad");
                btnImage.sprite = btnSprites[imageIndex + 5];
                imageIndex += 5;
            } else
            {
                Debug.Log("Setting to kid");
                btnImage.sprite = btnSprites[imageIndex - 5];
                imageIndex -= 5;
            }
            manager.ToggleMini(player);
        } /*else  // this will enable setting ai to dads
        {
            miniImage.SetActive(!miniImage.activeSelf);
            manager.ToggleMini(player);
        }*/
    }

    public void SetAI(bool setAI)
    {
        aiImage.SetActive(setAI);
    }
}
