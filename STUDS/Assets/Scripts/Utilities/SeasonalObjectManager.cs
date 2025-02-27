using UnityEngine;
using System;

public class SeasonalObjectManager : MonoBehaviour
{
    [Header("Seasonal Objects")]
    public GameObject springObjects;
    public GameObject summerObjects;
    public GameObject fallObjects;
    public GameObject winterObjects;

    void Start()
    {
        EnableSeasonalObjects();
    }

    void EnableSeasonalObjects()
    {
        int month = DateTime.Now.Month; // Get current month

        // Disable all objects first
        springObjects?.SetActive(false);
        summerObjects?.SetActive(false);
        fallObjects?.SetActive(false);
        winterObjects?.SetActive(false);

        // Enable the correct season's objects
        if (month >= 3 && month <= 5) // March - May (Spring)
        {
            springObjects?.SetActive(true);
        }
        else if (month >= 6 && month <= 8) // June - August (Summer)
        {
            summerObjects?.SetActive(true);
        }
        else if (month >= 9 && month <= 11) // September - November (Fall)
        {
            fallObjects?.SetActive(true);
        }
        else // December - February (Winter)
        {
            winterObjects?.SetActive(true);
        }
    }
}
