using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartTimerOnSceneLoad : MonoBehaviour
{
    /// <summary>
    /// This script will start a timer attached to a game object called LoadGameScene
    /// </summary>
    private void OnLevelWasLoaded(int level)
    {
        GameObject.Find("LoadGameScene").GetComponent<TimerEvents>().StartTimer();
    }

}
