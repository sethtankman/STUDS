using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

public class NetDodgeballEventsManager : NetworkBehaviour
{
    [Header("Events")]
    public float EventCountdownTimer = 45.0f;
    public bool EventStarted = false;
    public bool EventEnded = false;
    public TextMeshProUGUI DodgeballUIText;
    public float EventDuration = 90.0f;
    public float EndingCountdownDuration = 5.0f;

    [Header("Colors")]
    public Color lightEventColor = Color.yellow;
    public Color heavyEventColor = Color.red;
    public Color randomEventColor = new Color(0.5f, 0, 0.5f);

    [Header("UI Text Sizes")]
    public int eventHeaderSize = 18;
    public int eventTypeSize = 28;

    [Header("Level Object Text Sizes")]
    public int levelObjectHeaderSize = 16;
    public int levelObjectTypeSize = 24;

    [Header("All Dodgeball Holder Prefab Refs")]
    public GameObject LightDodgeballHolders;
    public GameObject MediumDodgeballHolders;
    public GameObject HeavyDodgeballHolders;
    public GameObject RandomDodgeballHolders;

    [Header("Level Text Objects")]
    public List<GameObject> LevelTextObjects;

    [Header("Particle Effect")]
    // Prefab for the particle effect to instantiate
    public GameObject destructionParticleEffect;

    private enum DodgeballEventType
    {
        Light = 1,
        Heavy = 2,
        Random = 3
    }

    private DodgeballEventType currentEventType;
    private Color eventColor = Color.white;
    private float eventEndTimer;

    void Start()
    {
        StartCoroutine(CountdownCoroutine());
        LightDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);
    }

    IEnumerator CountdownCoroutine()
    {
        while (EventCountdownTimer > 0)
        {
            if (EventCountdownTimer <= 5.0f)
            {
                UpdateTimerText();
            }
            yield return new WaitForSeconds(1);
            EventCountdownTimer -= 1;
        }

        if (!EventStarted)
        {
            DodgeballEventPicker();
            eventEndTimer = EventDuration;
            StartCoroutine(EventEndCountdownCoroutine());
        }
    }

    IEnumerator EventEndCountdownCoroutine()
    {
        while (eventEndTimer > 0)
        {
            if (eventEndTimer <= EndingCountdownDuration)
            {
                UpdateEventEndText();
            }
            yield return new WaitForSeconds(1);
            eventEndTimer -= 1;
        }

        EndDodgeballEvent();
    }

    // Method to destroy grabbable objects with collision enabled and spawn particle effects
    void DestroyGrabbablesWithCollision()
    {
        GameObject[] grabbableObjects = GameObject.FindGameObjectsWithTag("Grabbable");

        foreach (GameObject obj in grabbableObjects)
        {
            Collider collider = obj.GetComponent<Collider>();
            if (collider != null && collider.enabled)
            {
                // Instantiate particle effect at the object's position and rotation
                Instantiate(destructionParticleEffect, obj.transform.position, obj.transform.rotation);

                // Destroy the object
                if (isServer)
                    NetworkServer.Destroy(obj);
            }
        }
    }

    void DodgeballEventPicker()
    {
        // Add a 0.1 second delay before starting the event and destroying grabbable objects
        Invoke(nameof(DestroyGrabbablesWithCollision), 0.1f);

        currentEventType = (DodgeballEventType)Random.Range(1, 4);
        EventStarted = true;
        HandleDodgeballEvent(currentEventType);
    }

    void HandleDodgeballEvent(DodgeballEventType eventType)
    {
        LightDodgeballHolders.SetActive(false);
        MediumDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);
        GameObject ChosenHolders = MediumDodgeballHolders;
        switch (eventType)
        {
            case DodgeballEventType.Light:
                ChosenHolders = LightDodgeballHolders;
                eventColor = lightEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>LIGHT DODGEBALLS</color></size>");
                break;
            case DodgeballEventType.Heavy:
                ChosenHolders = HeavyDodgeballHolders;
                eventColor = heavyEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>HEAVY DODGEBALLS</color></size>");
                break;
            case DodgeballEventType.Random:
                ChosenHolders = RandomDodgeballHolders;
                eventColor = randomEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>RANDOM DODGEBALLS</color></size>");
                break;
        }
        ChosenHolders.SetActive(true);
        foreach (TimerEvents t in ChosenHolders.GetComponentsInChildren<TimerEvents>())
        {
            t.SetTimer(t.duration - 1);
            t.StartTimer();
        }
    }

    void SetEventText(string newText)
    {
        // Set UI text
        DodgeballUIText.text = newText;

        // Independent text for level objects
        string levelObjectText = $"<size={levelObjectHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={levelObjectTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>{GetEventTypeText()}</color></size>";

        foreach (GameObject obj in LevelTextObjects)
        {
            TextMeshPro textComponent = obj.GetComponent<TextMeshPro>();
            if (textComponent != null)
            {
                textComponent.text = levelObjectText;
            }
        }
    }

    string GetEventTypeText()
    {
        switch (currentEventType)
        {
            case DodgeballEventType.Light:
                return "LIGHT DODGEBALLS";
            case DodgeballEventType.Heavy:
                return "HEAVY DODGEBALLS";
            case DodgeballEventType.Random:
                return "RANDOM DODGEBALLS";
            default:
                return "";
        }
    }

    void EndDodgeballEvent()
    {
        EventEnded = true;

        // Destroy all grabbable objects with collisions when the event ends
        DestroyGrabbablesWithCollision();

        LightDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);
        MediumDodgeballHolders.SetActive(true);
        SetEventText("");
    }

    void UpdateTimerText()
    {
        int timeInSeconds = Mathf.CeilToInt(EventCountdownTimer);
        string countdownText = timeInSeconds == 1 ? "1 second" : $"{timeInSeconds} seconds";

        SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT STARTS IN</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>{countdownText}</color></size>");
    }

    void UpdateEventEndText()
    {
        int timeInSeconds = Mathf.CeilToInt(eventEndTimer);
        string countdownText = timeInSeconds == 1 ? "1 second" : $"{timeInSeconds} seconds";

        SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ENDS IN</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>{countdownText}</color></size>");
    }
}
