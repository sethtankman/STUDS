using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DadBtn : MonoBehaviour
{
    public Text btnText;
    public Sprite btnSprite; //This will be used once we have the eugine images.
    public GameObject player; //So the button can be connected to the player.
    public DadButtonMngr manager;

    private void Start()
    {
        manager = GameObject.Find("Canvas").GetComponent<DadButtonMngr>();
    }

    public void RandomPlayer()
    {
        ManagePlayerHub hub = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>();
        List<GameObject> players = hub.getPlayers();
        int selected = (int)(Random.value * players.Count);
        int i = 0;
        foreach (GameObject _player in players) {
            if(selected == i)
            {
                player = _player;
            }
        }

        SetupPlayers();
    }

    public void SetText(string newText)
    {
        btnText.text = newText;
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
