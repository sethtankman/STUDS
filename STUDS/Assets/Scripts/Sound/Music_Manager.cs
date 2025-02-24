﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music_Manager : MonoBehaviour
{
    // Start is called before the first frame update

    public AK.Wwise.Event menuMusic;
    public AK.Wwise.Event strollerMusic;
    public AK.Wwise.Event shoppingMusic;
    public AK.Wwise.Event pennyMusic;
    public AK.Wwise.Event dodgeballMusic;

    private static GameObject instance;

    void Start()
    {
        if (instance == null)
        {
            instance = this.gameObject;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(this.gameObject);
    }

    public void PlayStopMusic(string soundName, bool play)
    {
        if (play)
        {
            switch (soundName)
            {
                case "Menu":
                    menuMusic.Post(gameObject);
                    break;
                case "Stroller":
                    strollerMusic.Post(gameObject);
                    break;
                case "Shopping":
                    shoppingMusic.Post(gameObject);
                    break;
                case "Penny":
                    pennyMusic.Post(gameObject);
                    break;
                case "Dodgeball":
                    dodgeballMusic.Post(gameObject); 
                    break;
                default:
                    Debug.LogError("Yo, that's not the music's name.");
                    break;
            }
        } else
        {
            switch (soundName)
            {
                case "Menu":
                    menuMusic.Stop(gameObject);
                    break;
                case "Stroller":
                    strollerMusic.Stop(gameObject);
                    break;
                case "Shopping":
                    shoppingMusic.Stop(gameObject);
                    break;
                case "Penny":
                    pennyMusic.Stop(gameObject);
                    break;
                case "Dodgeball":
                    dodgeballMusic.Stop(gameObject);
                    break;
                default:
                    Debug.LogError("Yo, that's not the music's name.");
                    break;
            }
        }
    }
}
