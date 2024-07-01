using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCharacter : MonoBehaviour
{
    public List<GameObject> players;
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
        foundMatch = false;
        isPowerBill = (GameObject.Find("PBFinalText(Clone)") != null);
        players = ManagePlayerHub.Instance.getPlayers();
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
        if (isPowerBill && player.GetComponent<CharacterMovementController>().isMini)
        {
            gameObject.GetComponent<CharacterMovementController>().SetToMini(true);
            //gameObject.GetComponent<CharacterMovementController>().enabled = false;
            // Not sure why we disable cmcs in this method.
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
