using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RoomButton : Button
{
    public CSteamID SteamId;
    public Button JoinButton;

    [SerializeField] private Text RoomNameText;
    [SerializeField] private Text NumPlayersText;

    public void SetupButton(Dictionary<string, string> dict)
    {
        foreach (var key in dict.Keys)
        {
            Debug.Log("Key: " + key);
        }
        RoomNameText.text = dict["RoomName"];
        NumPlayersText.text = dict["NumPlayers"] + "/4"; //If numplayers = 4, disable button.

    }

    public void SetLobbyItemValues(string RoomName, int numberOfPlayers, int maxNumberOfPlayers)
    {
        JoinButton = GameObject.Find("JoinButton").GetComponent<Button>();
        RoomNameText.text = RoomName;
        NumPlayersText.text = "Number of Players: " + numberOfPlayers.ToString() + "/" + maxNumberOfPlayers.ToString();
    }

    public void SetJoinButtonInteractable()
    {
        JoinButton.interactable = true;
        NetworkMenuActions.instance.SelectedRoomId = SteamId;
    }
}
