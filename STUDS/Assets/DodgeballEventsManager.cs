using System.Collections;
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
    public Color lightEventColor = Color.yellow; // Yellow for light event
    public Color heavyEventColor = Color.red;    // Red for heavy event
    public Color randomEventColor = new Color(0.5f, 0, 0.5f); // Purple for random event

    [Header("Text Sizes")]
    public int eventHeaderSize = 18;   // Size for "EVENT ACTIVE"
    public int eventTypeSize = 28;     // Size for event type (e.g., "HEAVY DODGEBALLS")
    //public int countdownSize = 28;     // Size for countdown text (e.g., "5 seconds")
    //public int lastFiveSecondsSize = 28; // Size for last 5 seconds countdown

    [Header("All Dodgeball Holder Prefab Refs")]
    public GameObject LightDodgeballHolders;
    public GameObject MediumDodgeballHolders;
    public GameObject HeavyDodgeballHolders;
    public GameObject RandomDodgeballHolders;

    private enum DodgeballEventType
    {
        Light = 1,
        Heavy = 2,
        Random = 3
    }

    private DodgeballEventType currentEventType;
    private Color eventColor = Color.white; // Default color
    private float eventEndTimer; // Timer for last 5 seconds

    void Start()
    {
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        // Main countdown timer
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
            eventEndTimer = EventDuration; // Initialize end timer
            StartCoroutine(EventEndCountdownCoroutine());
        }
    }

    IEnumerator EventEndCountdownCoroutine()
    {
        // Event duration timer
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
        // Deactivate all holders
        LightDodgeballHolders.SetActive(false);
        MediumDodgeballHolders.SetActive(false);
        HeavyDodgeballHolders.SetActive(false);
        RandomDodgeballHolders.SetActive(false);

        // Set eventText and eventColor based on eventType
        switch (eventType)
        {
            case DodgeballEventType.Light:
                LightDodgeballHolders.SetActive(true);
                eventColor = lightEventColor;
                DodgeballUIText.text = $"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>LIGHT DODGEBALLS</color></size>";
                break;
            case DodgeballEventType.Heavy:
                HeavyDodgeballHolders.SetActive(true);
                eventColor = heavyEventColor;
                DodgeballUIText.text = $"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>HEAVY DODGEBALLS</color></size>";
                break;
            case DodgeballEventType.Random:
                RandomDodgeballHolders.SetActive(true);
                eventColor = randomEventColor;
                DodgeballUIText.text = $"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ACTIVE</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>RANDOM DODGEBALLS</color></size>";
                break;
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
        DodgeballUIText.text = "";
    }

    void UpdateTimerText()
    {
        int timeInSeconds = Mathf.CeilToInt(EventCountdownTimer);
        string countdownText = timeInSeconds == 1 ? "1 second" : $"{timeInSeconds} seconds";

        // Ensure correct size and color for "EVENT STARTS IN" and countdown
        DodgeballUIText.text = $"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT STARTS IN</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>{countdownText}</color></size>";
    }

    void UpdateEventEndText()
    {
        int timeInSeconds = Mathf.CeilToInt(eventEndTimer);
        string countdownText = timeInSeconds == 1 ? "1 second" : $"{timeInSeconds} seconds";

        // Update text with last 5 seconds countdown
        DodgeballUIText.text = $"<size={eventHeaderSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>EVENT ENDS IN</color></size>\n<size={eventTypeSize}><color=#{ColorUtility.ToHtmlStringRGB(eventColor)}>{countdownText}</color></size>";
    }
}