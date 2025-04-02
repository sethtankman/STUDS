using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetVictoryCharacter : NetworkBehaviour
{
    public GameObject[] players;
    [SerializeField] private GameObject PolySurface;
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
        if(isServer)
            StartCoroutine(nameof(FindMatchingPlayer));
    }

    private IEnumerator FindMatchingPlayer()
    {
        while (foundMatch == false)
        {
            foundMatch = false;
            isPowerBill = (GameObject.Find("PBFinalText(Clone)") != null);
            players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                GetComponent<CharacterMovementController>().enabled = true;
                if (player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == posNumber)
                {
                    RpcActivateCharacter(player.GetComponent<NetworkCharacterMovementController>().GetColorName(), isPowerBill && player.GetComponent<NetworkCharacterMovementController>().isMini);
                    foundMatch = true;
                    break;
                }
            }

            if (!foundMatch)
            {
                RpcDeactivateCharacter();
            }
            yield return new WaitForSeconds(1f);
        }
    }

    [ClientRpc]
    private void RpcActivateCharacter(string colorName, bool turnMini)
    {
        SetColor(colorName);
        if (turnMini) { gameObject.GetComponent<CharacterMovementController>().SetToMini(true); }
        PolySurface.SetActive(true);
    }

    [ClientRpc]
    private void RpcDeactivateCharacter()
    {
        PolySurface.SetActive(false);
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
