using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerConnection : MonoBehaviour
{
    public ManagePlayerHub playerHub;

    public Image[] playerImages;
    public GameObject[] textObjects;

    public void Start()
    {
        playerHub = GameObject.Find("GameManager").GetComponent<ManagePlayerHub>();
    }

    public void SetPanelImage(int panelID, string colorName)
    {
        playerImages[panelID].color = Color.white;
        Debug.Log("This is my color: " + colorName);
        textObjects[panelID].GetComponent<TextMeshProUGUI>().text = "Player " + (panelID+1);
    }

}
