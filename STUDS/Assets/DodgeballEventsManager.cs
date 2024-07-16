using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DodgeballEventsManager : MonoBehaviour
{
    [Header("Events")]
    public float EventCountdownTimer = 45.0f;
    public bool EventStarted = false;
    public bool EventEnded = false;
    public float RandomEventSelect = 0;

    [Header("All Dodgeball Holder Prefab Refs")]
    public GameObject LightDodgeballHolders;
    public GameObject MediumDodgeballHolders;
    public GameObject HeavyDodgeballHolders;
    public GameObject RandomDodgeballHolders;

    // Update is called once per frame
    void Update()
    {
        EventCountdownTimer -= Time.deltaTime;
        if(EventCountdownTimer <= 0.0f)
        {
            if (!EventStarted)
            {
                DodgeballEventPicker();
                StartCoroutine(EndEventTimer(90));
            }
        }
    }

    //Event Types
    void DodgeballEventLight()
    {
        MediumDodgeballHolders.SetActive(false);
        LightDodgeballHolders.SetActive(true);
    }

    void DodgeballEventHeavy()
    {
        MediumDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(true);
    }

    void DodgeballEventRandom()
    {
        MediumDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(true);
    }

    //Random pick event
    void DodgeballEventPicker()
    {
        RandomEventSelect = Random.Range(1, 3);
        EventStarted = true;

        if (RandomEventSelect == 1)
        {
            DodgeballEventLight();
        }
        if(RandomEventSelect == 2)
        {
            DodgeballEventHeavy();
        }
        if(RandomEventSelect == 3)
        {
            DodgeballEventRandom();
        }
    }

    void EndDodgeballEvent()
    {
        EventEnded = true;
        LightDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);
        MediumDodgeballHolders.SetActive(true);
    }

    IEnumerator EndEventTimer(float time)
    {
        yield return new WaitForSeconds(time);

        EndDodgeballEvent();
    }
}
