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
    private int playersLoaded = 0;
    [SerializeField] private Button LevelSelectBtn;
    [SerializeField] private Camera MainCam, LoadingCam;

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
        if (isServer)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<NetworkCharacterMovementController>().isAI)
                {
                    if (player.GetComponent<NetPlayerAI>()) { player.GetComponent<NetPlayerAI>().enabled = false; }
                    player.GetComponent<CharacterController>().enabled = false;
                    player.transform.position = new Vector3(0, 0, 60);
                    player.GetComponent<CharacterController>().enabled = true;
                }
            }
        }
    }

    public void NotifyPlayerReady()
    {
        playersLoaded++;
        if (isServer && playersLoaded == NetGameManager.Instance.playerIDCount)
        {
            RpcInitVictory();
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

    [ClientRpc]
    private void RpcInitVictory()
    {
        NetVictoryCharacter[] vcs = FindObjectsByType<NetVictoryCharacter>(FindObjectsSortMode.None);
        foreach (NetVictoryCharacter vc in vcs)
        {
            vc.FindMatchingPlayer();
        }
        LoadingCam.enabled = false;
        MainCam.enabled = true;
    }
}
