using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class NetDadBtn : NetworkBehaviour
{
    public Text btnText;
    public Image btnImage;
    public Material[] kidMaterials;
    public Sprite[] characterSprites;
    public GameObject player, aiImage; //So the button can be connected to the player.
    public NetDadBtnMngr manager;
    public string color;
    public int imageIndex;

    private void Start()
    {
        manager = GameObject.Find("Panel").GetComponent<NetDadBtnMngr>();
        if(!isServer)
        {
            transform.SetParent(manager.transform);
            //manager.CmdRequestBtnInfo(this);  
        }
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

    [ClientRpc]
    public void RpcToggleMini()
    {
        ToggleMini();
    }

    [ClientRpc]
    public void RpcSetSprite(string colorName, bool isKid)
    {
        SetSprite(colorName, isKid);
    }

    [ClientRpc]
    public void RpcSetPlayer(GameObject player)
    {
        SetPlayer(player);
    }

    /// <summary>
    /// Sets the sprite to the one specified.  
    /// </summary>
    /// <param name="colorNum"></param>
    public void SetSprite(string colorName, bool isKid)
    {
        colorName = colorName.ToLower();
        color = colorName;
        // Sets the image index to the correct sprite if it is a kid.
        int kidOffset = 0;
        if(isKid) { kidOffset = 4; }
        switch (colorName)
        {
            case "blue":
                btnImage.sprite = characterSprites[kidOffset];
                imageIndex = kidOffset;
                break;
            case "green":
                btnImage.sprite = characterSprites[1 + kidOffset];
                imageIndex = 1 + kidOffset;
                break;
            case "red":
                btnImage.sprite = characterSprites[2 + kidOffset];
                imageIndex = 2 + kidOffset;
                break;
            case "purple":
                btnImage.sprite = characterSprites[3 + kidOffset];
                imageIndex = 3 + kidOffset;
                break;
            case "yellow":
                btnImage.sprite = characterSprites[4 + kidOffset];
                imageIndex = 4 + kidOffset;
                break;
            default:
                Debug.LogError("Unable to set sprite to specified color: " + colorName);
                break;
        }
        Debug.Log($"Image set to index {imageIndex}");
    }

    public void SetPlayer(GameObject refPlayer)
    {
        Debug.Log($"Setting player: {refPlayer}");
        player = refPlayer;
        // GetComponent<Button>().onClick.AddListener(ToggleMini);
    }

    
    public void ToggleMini()
    {
        if (aiImage.activeSelf == false)
        {

            if (imageIndex > 4)
            {
                Debug.Log("Setting to dad");
                btnImage.sprite = characterSprites[imageIndex - 5];
                imageIndex -= 5;
            }
            else
            {
                Debug.Log("Setting to kid");
                btnImage.sprite = characterSprites[imageIndex + 5];
                imageIndex += 5;
            }
            manager.ToggleMini(player);
        } /*else  // this will enable setting ai to dads
        {
            miniImage.SetActive(!miniImage.activeSelf);
            manager.ToggleMini(player);
        }*/
    }

    [ClientRpc]
    public void RpcSetAI(bool setAI)
    {
        SetAI(setAI);
    }

    public void SetAI(bool setAI)
    {
        aiImage.SetActive(setAI);
        btnImage.sprite = characterSprites[imageIndex + 5];
        imageIndex += 5;
    }

    [ClientRpc]
    public void RpcSetPivot(Vector2 _pivot)
    {
        Debug.Log($"Pivot set to {_pivot}");
        GetComponent<RectTransform>().pivot = _pivot;
        if (!manager)
        {
            manager = FindObjectOfType<NetDadBtnMngr>();
        }
    }
}
