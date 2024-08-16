using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DodgeballEventsManager : MonoBehaviour
{
    [Header("Events")]
    public float EventCountdownTimer = 45.0f;
    public bool EventStarted = false;
    public bool EventEnded = false;
    public float RandomEventSelect = 0;
    public TextMeshProUGUI DodgeballUIText;

    [Header("All Dodgeball Holder Prefab Refs")]
    public GameObject LightDodgeballHolders;
    public GameObject MediumDodgeballHolders;
    public GameObject HeavyDodgeballHolders;
    public GameObject RandomDodgeballHolders;

    // Update is called once per frame
    void Update()
    {
        EventCountdownTimer -= Time.deltaTime;
        if (EventCountdownTimer <= 0.0f)
        {
            if (!EventStarted)
            {
                DodgeballEventPicker();
                StartCoroutine(EndEventTimer(90));
            }
        }

        if (EventCountdownTimer <= 5.0f)
        {
            if (!EventStarted)
            {
                UpdateTimerText();
            }
        }
    }

    // Event Types
    void DodgeballEventLight()
    {
        MediumDodgeballHolders.SetActive(false);
        LightDodgeballHolders.SetActive(true);
        DodgeballUIText.text = "EVENT ACTIVE\n LIGHT DODGEBALLS";
    }

    void DodgeballEventHeavy()
    {
        MediumDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(true);
        DodgeballUIText.text = "EVENT ACTIVE\n HEAVY DODGEBALLS";
    }

    void DodgeballEventRandom()
    {
        MediumDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(true);
        DodgeballUIText.text = "EVENT ACTIVE\n RANDOM DODGEBALLS";
    }

    // Random pick event
    void DodgeballEventPicker()
    {
        RandomEventSelect = Random.Range(1, 4);
        EventStarted = true;

        if (RandomEventSelect == 1)
        {
            DodgeballEventLight();
        }
        if (RandomEventSelect == 2)
        {
            DodgeballEventHeavy();
        }
        if (RandomEventSelect == 3)
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
        DodgeballUIText.text = "";
    }

    void UpdateTimerText()
    {
        int timeInSeconds = Mathf.CeilToInt(EventCountdownTimer);

        DodgeballUIText.text = "EVENT STARTS IN\n" + timeInSeconds + " SECONDS";
    }

    IEnumerator EndEventTimer(float time)
    {
        yield return new WaitForSeconds(time);

        EndDodgeballEvent();
    }
}