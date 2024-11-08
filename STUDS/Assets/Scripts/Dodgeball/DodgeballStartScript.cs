using UnityEngine;
using TMPro;
using System.Collections;

public class DodgeballStartScript : MonoBehaviour
{
    // References to the TextMeshProUGUI components for displaying active child names
    public TextMeshProUGUI timeOfDayText;
    public TextMeshProUGUI weatherEffectsText;

    // References to the parent GameObjects
    public GameObject timeOfDayLighting;
    public GameObject weatherEffects;

    private void Start()
    {
        if (timeOfDayLighting != null && weatherEffects != null)
        {
            // Clear previous texts
            timeOfDayText.text = "";
            weatherEffectsText.text = "";

            // Start coroutine to process active children after a short delay
            StartCoroutine(ProcessChildrenAfterDelay());
        }
        else
        {
            Debug.LogError("Parent GameObjects are not assigned.");
        }
    }

    private IEnumerator ProcessChildrenAfterDelay()
    {
        // Wait for a short duration
        yield return new WaitForEndOfFrame();

        // Process and display active children for TimeOfDay/Lighting
        ProcessActiveTimeOfDayChildren(timeOfDayLighting, timeOfDayText);

        // Process and display active children for Weather/Effects
        ProcessActiveWeatherChildren(weatherEffects, weatherEffectsText);
    }

    private void ProcessActiveTimeOfDayChildren(GameObject parent, TextMeshProUGUI uiText)
    {
        // Check if the UI TextMeshPro component is assigned
        if (uiText == null)
        {
            Debug.LogError("UI TextMeshProUGUI component is not assigned.");
            return;
        }

        // Log parent GameObject information
        Debug.Log("Processing active children for TimeOfDay/Lighting");

        // Iterate through each child of the parent GameObject
        foreach (Transform child in parent.transform)
        {
            bool isActive = child.gameObject.activeInHierarchy; // Use activeInHierarchy
            Debug.Log("Child name: " + child.name + " | Active In Hierarchy: " + isActive);

            if (isActive)
            {
                // Set UI text based on the active child name
                switch (child.name)
                {
                    case "Day":
                        uiText.text = "11:00 AM";
                        break;
                    case "Sunset":
                        uiText.text = "6:30 PM";
                        break;
                    case "Night":
                        uiText.text = "8:00 PM";
                        break;
                    default:
                        uiText.text = "Unknown Time";
                        break;
                }
            }
        }

        if (string.IsNullOrEmpty(uiText.text))
        {
            Debug.LogWarning("No active TimeOfDay children found or text is empty.");
        }
    }

    private void ProcessActiveWeatherChildren(GameObject parent, TextMeshProUGUI uiText)
    {
        // Check if the UI TextMeshPro component is assigned
        if (uiText == null)
        {
            Debug.LogError("UI TextMeshProUGUI component is not assigned.");
            return;
        }

        // Track the active states of Rain and Fog
        bool isRainActive = false;
        bool isFogActive = false;

        // Iterate through each child of the parent GameObject
        foreach (Transform child in parent.transform)
        {
            bool isActive = child.gameObject.activeInHierarchy; // Use activeInHierarchy

            if (child.name == "Rain")
            {
                isRainActive = isActive;
            }
            else if (child.name == "Fog")
            {
                isFogActive = isActive;
            }
        }

        // Set UI text based on the active states of Rain and Fog
        if (isRainActive && isFogActive)
        {
            uiText.text = "STORMY";
        }
        else if (!isRainActive && !isFogActive)
        {
            uiText.text = "CLEAR SKIES";
        }
        else if (isRainActive)
        {
            uiText.text = "RAINING";
        }
        else if (isFogActive)
        {
            uiText.text = "FOGGY";
        }

        if (string.IsNullOrEmpty(uiText.text))
        {
            Debug.LogWarning("No active Weather children found or text is empty.");
        }
    }
}