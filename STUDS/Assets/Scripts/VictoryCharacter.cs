using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCharacter : MonoBehaviour
{
    List<GameObject> players;
    public int posNumber;
    public Material color1;
    public Material color2;
    public Material color3;
    public Material color4;
    public Material color5;
    private bool foundMatch, isPowerBill;
    public bool networkMode;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        foundMatch = false;
        isPowerBill = (GameObject.Find("PBFinalText(Clone)") != null);
        if (!networkMode)
        {
            players = ManagePlayerHub.Instance.getPlayers();
        }
        else
        {
            players = NetGameManager.Instance.getPlayers();
        }
        if (networkMode)
        {
            foreach (GameObject player in players)
            {
                gameObject.GetComponent<CharacterMovementController>().enabled = true;
                if (player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == posNumber)
                {
                    SetColor(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
                    if (foundMatch == false)
                    {
                        TurnMini(player);
                    }
                    foundMatch = true;
                    break;
                }
            }
        }
        else
        {
            foreach (GameObject player in players)
            {
                gameObject.GetComponent<CharacterMovementController>().enabled = true;
                if (player.GetComponent<CharacterMovementController>().GetFinishPosition() == posNumber)
                {
                    SetColor(player.GetComponent<CharacterMovementController>().GetColorName());
                    if (foundMatch == false)
                        TurnMini(player);
                    foundMatch = true;
                }
            }
        }
        if (!foundMatch)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Turns player mini if isMini is true
    /// </summary>
    /// <param name="player"></param>
    private void TurnMini(GameObject player)
    {
        //Debug.Log("Calling TurnMini " + isPowerBill + "  " + player.GetComponent<CharacterMovementController>().isMini);
        if (player.GetComponent<NetworkCharacterMovementController>()) {
            if (isPowerBill && player.GetComponent<NetworkCharacterMovementController>().isMini)
            {
                gameObject.GetComponent<CharacterMovementController>().SetToMini(true);
                gameObject.GetComponent<CharacterMovementController>().enabled = false;
            }
        }
        else if (isPowerBill && player.GetComponent<CharacterMovementController>().isMini)
        {
            gameObject.GetComponent<CharacterMovementController>().SetToMini(true);
            gameObject.GetComponent<CharacterMovementController>().enabled = false;
        }
    }

    private void SetColor(string colorName)
    {
        Debug.Log("Finished player should be: " + colorName);
        if (colorName.Equals("blue"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color1;
        }
        else if (colorName.Equals("green"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color2;
        }
        else if (colorName.Equals("red"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color3;
        }
        else if (colorName.Equals("yellow"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color4;
        }
        else if (colorName.Equals("purple"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color5;
        }
    }
}
