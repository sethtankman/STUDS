using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P4Placeholder : MonoBehaviour
{
    private int numPLR;
    public bool PlaceholderOn = false;

    public GameObject P4PH;
    public RectTransform image;

    // Start is called before the first frame update
    void Start()
    {
       
        P4PH.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        numPLR = 0;
        if (!PlaceholderOn) {
            foreach (GameObject Player in GameObject.FindGameObjectsWithTag("Player"))
            {
                numPLR += 1;
            }

            if (numPLR == 3)
            {
                image.sizeDelta = new Vector2( Screen.width / 2,  Screen.height / 2);
                
                PlaceholderOn = true;
                P4PH.SetActive(true);
            }
            else
            {
                PlaceholderOn = false;
                P4PH.SetActive(false);
            }
        }
        

    }

    public void AddPlr() { 
    }
}
