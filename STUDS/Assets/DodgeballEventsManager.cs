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

    void HandleDodgeballEvent(DodgeballEventType eventType)
    {
        LightDodgeballHolders.SetActive(false);
        MediumDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);

        switch (eventType)
        {
            case DodgeballEventType.Light:
                LightDodgeballHolders.SetActive(true);
                eventColor = lightEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>LIGHT DODGEBALLS</color></size>");
                break;
            case DodgeballEventType.Heavy:
                HeavyDodgeballHolders.SetActive(true);
                eventColor = heavyEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>HEAVY DODGEBALLS</color></size>");
                break;
            case DodgeballEventType.Random:
                RandomDodgeballHolders.SetActive(true);
                eventColor = randomEventColor;
                SetEventText($"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>RANDOM DODGEBALLS</color></size>");
                break;
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

    void DodgeballEventPicker()
    {
        currentEventType = (DodgeballEventType)Random.Range(1, 4);
        EventStarted = true;
        HandleDodgeballEvent(currentEventType);
    }

    void EndDodgeballEvent()
    {
        EventEnded = true;
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