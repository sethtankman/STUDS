using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeTimeOfDay : MonoBehaviour
{
    public GameObject Day;
    public GameObject Sunset;
    public GameObject Night;
    public float randomNumber;

    void Start()
    {
        randomNumber = Random.Range(1, 4);

        if (randomNumber == 1)
        {
            Day.SetActive(true);
        }
        else if (randomNumber == 2)
        {
            Sunset.SetActive(true);
        }
        else
        {
            Night.SetActive(true);
        }
    }
}
