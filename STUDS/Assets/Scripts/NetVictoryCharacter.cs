using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetVictoryCharacter : MonoBehaviour
{
    public GameObject[] players;
    public int posNumber;
    public Material color1;
    public Material color2;
    public Material color3;
    public Material color4;
    public Material color5;
    [SerializeField] private bool foundMatch;
    private bool isPowerBill;
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(FindMatchingPlayer), 0.1f);
    }

    private void FindMatchingPlayer()
    {
        foundMatch = false;
        isPowerBill = (GameObject.Find("PBFinalText(Clone)") != null);
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            GetComponent<CharacterMovementController>().enabled = true;
            if (player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == posNumber)
            {
                SetColor(player.GetComponent<NetworkCharacterMovementController>().GetColorName());
                TurnMini(player);
                foundMatch = true;
                break;
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
        if (player.GetComponent<NetworkCharacterMovementController>()) {
            if (isPowerBill && player.GetComponent<NetworkCharacterMovementController>().isMini)
            {
                gameObject.GetComponent<CharacterMovementController>().SetToMini(true);
                //gameObject.GetComponent<CharacterMovementController>().enabled = false;
            }
        }
    }

    private void SetColor(string colorName)
    {
        colorName = colorName.ToLower();
        GetComponent<CharacterMovementController>().SetColorName(colorName);
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
