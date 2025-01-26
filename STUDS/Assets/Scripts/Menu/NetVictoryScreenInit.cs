using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using Mirror;
using UnityEngine.UI;

public class NetVictoryScreenInit : NetworkBehaviour
{
    public GameObject finalText;
    [SerializeField] private Button LevelSelectBtn;

    // Start is called before the first frame update
    void Start()
    {
        NetPause.canPause = false;
        Time.timeScale = 1f;
        if (!isServer)
            LevelSelectBtn.interactable = false;

        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Penny", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Dodgeball", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", true);


        finalText = GameObject.Find("PBFinalText(Clone)");
        if (finalText)
        {
            DetermineFinalText();
        }
    }

    private void DetermineFinalText()
    {
        finalText.transform.SetParent(GameObject.Find("Canvas").transform);
        double score = Double.Parse(finalText.GetComponent<TextMeshProUGUI>().text);
        finalText.GetComponent<TextMeshProUGUI>().text = "Power Bill: $" + (score / 10) + "0";
        finalText.GetComponent<RectTransform>().anchoredPosition = new Vector3(10, -10, 0);
        if(score > 4000)
        {
            finalText.GetComponent<TextMeshProUGUI>().text += " When your credit card is stolen but your credit starts to improve...";
        } else if (score > 3000)
        {
            finalText.GetComponent<TextMeshProUGUI>().text += " This is highway ROBBERY!!!";
        } else if (score > 2000)
        {
            finalText.GetComponent<TextMeshProUGUI>().text += " Put it on my tab.";
        } else if (score > 1000)
        {
            finalText.GetComponent<TextMeshProUGUI>().text += " Time for that new boat!";
        } else
        {
            SteamAchievements.UnlockAchievement("PB_ONLINE");
            finalText.GetComponent<TextMeshProUGUI>().text += " My mother-in-law will have nothing to say...";
        }
    }
}
