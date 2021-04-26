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
    private bool foundMatch;
    // Start is called before the first frame update
    void Start()
    {
        players = new List<GameObject>();
        foundMatch = false;
    }

    // Update is called once per frame
    void Update()
    {
        players = ManagePlayerHub.Instance.getPlayers();
        foreach (GameObject player in players)
        {
            Debug.Log("Player Pos: " + player.GetComponent<CharacterMovementController>().GetFinishPosition());
            if(player.GetComponent<CharacterMovementController>().GetFinishPosition() == posNumber)
            {
                SetColor(player.GetComponent<CharacterMovementController>().GetColorName());
                foundMatch = true;
            }
        }
        if (!foundMatch)
        {
            gameObject.SetActive(false);
        }
    }

    private void SetColor(string colorName)
    {
        Debug.Log("Finished player should be: " + colorName);
        if (colorName.Equals("Blue"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color1;
        }
        else if (colorName.Equals("Green"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color2;
        }
        else if (colorName.Equals("Red"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color3;
        }
        else if (colorName.Equals("Yellow"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color4;
        }
        else if (colorName.Equals("Purple"))
        {
            GetComponentInChildren<SkinnedMeshRenderer>().material = color5;
        }
    }
}
