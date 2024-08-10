using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ShoppingTimer : MonoBehaviour

{
    public TextMeshProUGUI TimerTXT;
    public float timer;
    private float Sprint = 10.0f;

    public int racePositions;
    public int noFinishPos;

    void Start()
    {
        racePositions = 1;
        noFinishPos = -1;
    }

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
        List<GameObject> players = ManagePlayerHub.Instance.getPlayers();
        foreach (GameObject player in players)
        {
            if (player.GetComponent<CharacterMovementController>().GetFinishPosition() == 0)
            {
                player.GetComponent<CharacterMovementController>().SetFinishPosition(noFinishPos);
                noFinishPos--;
            }
        }
        SteamAchievements.UnlockAchievement("SS_FINISH");
        SceneManager.LoadScene("VictoryStands");
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
