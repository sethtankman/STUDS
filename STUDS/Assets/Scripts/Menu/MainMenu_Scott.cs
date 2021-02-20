using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Scott : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GarageScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
