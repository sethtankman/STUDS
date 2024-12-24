using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTimeOfDay : MonoBehaviour
{
    //debug booleans
    [Header("Enable/Disable Debug")]
    [Tooltip("Enable manual selection of time, weather")]
    public bool debugEnabled = false;

    [Header("Debug Options")]
    public bool setDay = false;
    public bool setSunset = false;
    public bool setNight = false;
    public bool setRaining = false;
    public bool setFoggy = false;

    [Header("Randomized options")]
    [Tooltip("Enable randomized time, weather -- disables debug")]
    public bool randomEnabled = true;

    //random numbers
    [Header("Debug View Random Number Values")]
    [Tooltip("View the value of floats that determine time, weather at runtime")]
    public float RandomSky;
    public float RandomRain;
    public float RandomFog;

    //day, night, FX game objects
    [Header("Time and Weather GameObjects")]
    [Tooltip("Manually select time of day, weather")]
    public GameObject Day;
    public GameObject Sunset;
    public GameObject Night;
    public GameObject Rain;
    public GameObject Fog;

    void Start()
    {
        //set day, night, FX based on script booleans
        if (debugEnabled)
        {
            randomEnabled = false;

            if (setDay)
            {
                Day.SetActive(true);
            }
            else if (setSunset)
            {
                Sunset.SetActive(true);
            }
            else if (setNight)
            {
                Night.SetActive(true);
            }
            if (setRaining)
            {
                Rain.SetActive(true);
            }
            if (setFoggy)
            {
                Fog.SetActive(true);
            }

        }
        //set day, night, FX at random
        else if (randomEnabled)
        {
            DisableDebug();

            RandomSky = Random.Range(1, 4);
            RandomRain = Random.Range(0, 3); //33% chance of rain
            RandomFog = Random.Range(0, 4); //25% chance of fog

            //day
            if (RandomSky == 1)
            {
                Day.SetActive(true);
            }
            //sunset + weather
            else if (RandomSky == 2)
            {
                Sunset.SetActive(true);
                SetWeather();
            }
            //night + weather
            else
            {
                Night.SetActive(true);
                SetWeather();
            }
        }
    }

    void DisableDebug()
    {
        debugEnabled = false;
        setDay = false;
        setSunset = false;
        setNight = false;
        setFoggy = false;
        setRaining = false;
    }

    void SetWeather()
    {
        if (RandomFog == 1)
        {
            Fog.SetActive(true);
        }
        if (RandomRain == 1)
        {
            Rain.SetActive(true);
        }
    }
}
