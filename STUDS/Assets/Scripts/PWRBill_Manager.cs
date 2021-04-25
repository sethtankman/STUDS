using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PWRBill_Manager : MonoBehaviour
{
    //Electricity variables
    public float Score;
    public TextMeshProUGUI PowerTXT;
    public int NumItemsOn;
    public TextMeshProUGUI ItemsOnTXT;

    //List of objects to interact with
    public List<Interaction> Interactives = new List<Interaction>();
    public List<int> Validation = new List<int>();
    public int MaxObjectsOff;

    //Timer for the end of the game
    public TextMeshProUGUI TimerTXT;
    public float timer;
    private float Sprint = 10.0f;

    public GameObject PBGameEndText;

    // Start is called before the first frame update
    void Start()
    {

        foreach (GameObject Electronic in GameObject.FindGameObjectsWithTag("RandomPick"))
        {
            Interactives.Add(Electronic.GetComponent<Interaction>());
            if (Interactives.Contains(null))
            {
                Interactives.Remove(null);
            }
            
        }
        NumItemsOn = Interactives.Count;

        for (int j = 0; j < MaxObjectsOff; j++)
        {
            ValidatePicks();

        }

        for (int i = 0; i < MaxObjectsOff; i++)
        {
            //int tmp = Random.Range(0, Interactives.Count);
            
            Interactives[Validation[i]].ToggleVisualGM();
            print(Interactives[Validation[i]].name);

        }
    }

    // Update is called once per frame
    void Update()
    {
        Mathf.RoundToInt(timer);

        PowerTXT.text = "Power Bill: $" + (Score / 10) + "0";
        ItemsOnTXT.text = "Appliances: " + (NumItemsOn + 1);
               
        timer -= Time.deltaTime;

        if (timer > 0.0f)
        {
            Showtime(timer);
        }
        else
        {
            EndGame();
        }

    }

    public void AddScore(int toAdd)
    {
        Score += toAdd;

    }

    void EndGame()
    {
        GameObject EndText = Instantiate(PBGameEndText);
        DontDestroyOnLoad(EndText);
        EndText.GetComponent<TextMeshProUGUI>().text = "" + Score;
        SceneManager.LoadScene("VictoryStands");
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
