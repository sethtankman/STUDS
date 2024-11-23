using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwitcher : MonoBehaviour
{
    public string specificScene = "";

    public void LoadSpecificScene()
    {
        SceneManager.LoadScene(specificScene);
    }

    public void NextScene()
    {
        SceneManager.LoadScene("GarageScene");
    }

    public void PreviousScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
