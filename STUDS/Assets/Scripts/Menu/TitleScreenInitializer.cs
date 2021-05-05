using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TitleScreenInitializer : MonoBehaviour
{
    public GameObject finalText;
    public SteamAchievements sa;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;

        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Shopping", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Stroller", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Penny", false);
        GameObject.Find("Music Manager").GetComponent<Music_Manager>().PlayStopMusic("Menu", true);

        var players = ManagePlayerHub.Instance.getPlayers();
        foreach(GameObject player in players)
        {
            /*
            // Drop the players through the floor!
            player.transform.position = new Vector3(0, -10, 0);
            */
            // Destroying the player might be better...
            //Destroy(player);
            //Going to set them to inactive here and will delete them in my script after I have extracted their race positions
            player.SetActive(false);
        }

        finalText = GameObject.Find("PBFinalText(Clone)");
        sa = GameObject.Find("SteamAchievements").GetComponent<SteamAchievements>();
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
        finalText.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
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
            sa.UnlockAchievement("PB_THRIFTY");
            finalText.GetComponent<TextMeshProUGUI>().text += " My mother-in-law will have nothing to say...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
