using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SS_ItemTracker : MonoBehaviour
{

    private string[] itemList;
    private bool[] completedItemsCheck;
    public Text listText;
    // Start is called before the first frame update
    void Start()
    {
        itemList = new string[] {"Propane", "Sprinkler", "Toolbox", "Boombox", "Cooler", "SlowSign", "Helmet", "Shovel", "Hammer",};
        completedItemsCheck = new bool[itemList.Length];
        
    }

    // Update is called once per frame
    void Update()
    {
        bool allComplete = true;
        foreach(bool check in completedItemsCheck)
        {
            if (!check)
            {
                allComplete = false;
            }
        }
        if (allComplete)
        {
            listText.text = "You have successfully gathered all the items! You win!";
        }
        else
        {
            string text = "SHOPPING LIST:\n";
            for (int i = 0; i < itemList.Length; i++)
            {
                if (completedItemsCheck[i])
                {
                    text += itemList[i] + " - ✓\n";
                    Debug.Log("Added complete");
                }
                else
                {
                    //text += itemList[i] + ": Missing!\n";
                    text += itemList[i] + "\n";
                    Debug.Log("Added missing");
                }
            }
            listText.text = text;
        }

    }

    public string[] GetList()
    {
        return itemList;
    }

    public void CheckoutItem(int i)
    {
        completedItemsCheck[i] = true;
    }
}
