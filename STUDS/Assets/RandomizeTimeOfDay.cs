using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTimeOfDay : MonoBehaviour
{
    //gameobjects
    public GameObject Day;
    public GameObject Sunset;
    public GameObject Night;
    public GameObject Rain;
    public GameObject Fog;

    //random numbers
    public float RandomSky;
    public float RandomRain;
    public float RandomFog;

    void Start()
    {
        RandomSky = Random.Range(1, 4);
        RandomRain = Random.Range(1, 3);
        RandomFog = Random.Range(1, 3);

        if (RandomSky == 1)
        {
            Day.SetActive(true);
        }
        else if (RandomSky == 2)
        {
            Sunset.SetActive(true);
            if (RandomFog == 2)
            {
                Fog.SetActive(true);
            }
        }
        else
        {
            Night.SetActive(true);
            if (RandomRain == 2)
            {
                Rain.SetActive(true);
            }
            else if (RandomFog == 2)
            {
                Fog.SetActive(true);
            }
        }
    }
}
