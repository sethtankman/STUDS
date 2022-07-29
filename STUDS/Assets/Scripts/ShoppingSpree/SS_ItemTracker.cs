using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SS_ItemTracker : MonoBehaviour
{

    private string[] itemList;
    private bool[] completedItemsCheck;
    private Dictionary<string, bool> itemsCollected;
    public Image[] shoppingItemImages;
    public GameObject myPaper;
    public Text listText;
    public SteamAchievements sa;
    public bool isLocal = true;
    // Start is called before the first frame update
    void Start()
    {
        itemList = new string[] { "Boombox", "Helmet", "Cooler", "Hammer", "Shovel", "Sprinkler", "Toolbox", "Vacuum" };
        itemsCollected = new Dictionary<string, bool>();
        foreach (string item in itemList)
        {
            itemsCollected.Add(item, false);
        }
        completedItemsCheck = new bool[itemList.Length];
        sa = GameObject.Find("SteamAchievements").GetComponent<SteamAchievements>();
    }

    // Update is called once per frame
    void Update()
    {
        bool allComplete = true;
        foreach (bool check in completedItemsCheck)
        {
            if (!check)
            {
                allComplete = false;
            }
        }
        if (allComplete)
        {
            //listText.text = "You have successfully gathered all the items!";
            sa.UnlockAchievement("SS_FINISH");
        }
        //string text = "SHOPPING LIST:\n";
        for (int i = 0; i < itemList.Length; i++)
        {
            if (completedItemsCheck[i])
            {
                if (isLocal)
                {
                    shoppingItemImages[i].gameObject.GetComponent<ItemButton>().EnableCheckMark();
                    Debug.Log("Added complete");
                }
                //else
                //{
                    //Debug.Log("No shopping item images.  Intentional for non-local players.");
                //}
            }
            //else
            //{
                //Debug.Log("Added missing");
            //}
        }
        //listText.text = text;

    }

    public bool isItemCompleted(string item)
    {
        return itemsCollected[item];
    }

    public string[] GetList()
    {
        return itemList;
    }

    public void CheckoutItem(int i)
    {
        completedItemsCheck[i] = true;
        itemsCollected[itemList[i]] = true;
        Debug.Log("ID: " + i + ", itemList[i]: " + itemList[i]);
    }
}
