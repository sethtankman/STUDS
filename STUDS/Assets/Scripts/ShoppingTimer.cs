using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ShoppingTimer : MonoBehaviour

{
    public Text TimerTXT;
    public float timer;
    private float Sprint = 10.0f;

    void Update()
    {
        Mathf.RoundToInt(timer);

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

    void EndGame()
    {
        SceneManager.LoadScene("VictoryStands_PWR Bill");
    }

    void Showtime(float timeleft)
    {
        float min = Mathf.FloorToInt(timeleft / 60);
        float sec = Mathf.FloorToInt(timeleft % 60);

        if (sec == Sprint && min == 0.0f)
        {
            TimerTXT.fontSize += 10;
            Sprint -= 1.0f;
        }

        TimerTXT.text = string.Format("{0:00}:{1:00}", min, sec);

    }
}
