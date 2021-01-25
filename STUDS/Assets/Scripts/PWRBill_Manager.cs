using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PWRBill_Manager : MonoBehaviour
{
    //Electricity variables
    public int Score;
    public Text PowerTXT;
    public int NumItemsOn;
    public Text ItemsOnTXT;

    //List of objects to interact with
    public List<Interaction> Interactives = new List<Interaction>();
    private List<int> Validation = new List<int>();
    public int MaxObjectsOff;

    //Timer for the end of the game
    public Text TimerTXT;
    public float timer;
    private float Sprint = 10.0f;
   



    // Start is called before the first frame update
    void Start()
    {

        foreach (GameObject Electronic in GameObject.FindGameObjectsWithTag("RandomPick"))
        {
            Interactives.Add(Electronic.GetComponent<Interaction>());
            
        }
        NumItemsOn = Interactives.Count;

        for (int j = 0; j <= MaxObjectsOff; j++)
        {
            ValidatePicks();

        }

        for (int i = 0; i <= MaxObjectsOff; i++)
        {
            //int tmp = Random.Range(0, Interactives.Count);
            
            Interactives[Validation[i]].ToggleVisual();
     
        }
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.RoundToInt(timer);

        PowerTXT.text = "Power Bill: $" + Score;
        ItemsOnTXT.text = "Electronics: " + NumItemsOn;
        

        timer -= Time.deltaTime;

        if (timer > 0.0f)
        {
            Showtime(timer);
        }
        else
        {
            //End of round
        }

    }

    public void AddScore(int toAdd)
    {
        Score += toAdd;

    }

    void EndGame()
    {
        SceneManager.LoadScene("VictoryStands_PWR Bill");
    }

    void Showtime(float timeleft)
    {
        float min = Mathf.FloorToInt(timeleft / 60);
        float sec = Mathf.FloorToInt(timeleft % 60);

        if(sec == Sprint && min == 0.0f)
        {
            TimerTXT.fontSize += 10;
            Sprint -= 1.0f;
        }

        TimerTXT.text = string.Format("{0:00}:{1:00}", min, sec);       

    }

    void ValidatePicks()
    {
        int picked = Random.Range(0, Interactives.Count);
        while (Validation.Contains(picked))
        {
            picked = Random.Range(0, Interactives.Count);
        }
        Validation.Add(picked);

    }
}
