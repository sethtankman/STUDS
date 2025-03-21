using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : MonoBehaviour
{
    public float SpeedIncreaseMultiplier = 1.2f;
    public float BoostDuration = 3.0f; 
    public GameObject SpeedBoostEffectPrefab; 
    public GameObject speedupArrowPrefab; 

    private float speedupArrowsHeight = 2.25f; // Height to spawn arrows
    private Dictionary<GameObject, GameObject> activeArrows = new Dictionary<GameObject, GameObject>(); // Store arrows per player
    private Dictionary<GameObject, Coroutine> arrowCoroutines = new Dictionary<GameObject, Coroutine>(); // Store coroutines per player

    public void ActivateSpeedBoost(GameObject player)
    {
        CharacterMovementController cmc = player.GetComponent<CharacterMovementController>();
        NetworkCharacterMovementController ncmc = player.GetComponent<NetworkCharacterMovementController>();

        if (cmc)
        {
            StartCoroutine(ApplySpeedBoost(cmc, player));
        }
        else if (ncmc)
        {
            StartCoroutine(ApplySpeedBoost(ncmc, player));
        }
    }

    private IEnumerator ApplySpeedBoost(CharacterMovementController cmc, GameObject player)
    {
        if (cmc == null) yield break;

        // Apply speed boost
        cmc.setSpeedMultiplier(SpeedIncreaseMultiplier);
        Debug.Log("Speed Boost Applied: " + SpeedIncreaseMultiplier);

        // Spawn visual effect
        GameObject effect = Instantiate(SpeedBoostEffectPrefab, cmc.transform.position, Quaternion.identity);
        effect.transform.SetParent(cmc.transform);

        // Spawn speedup arrow effect
        GameObject arrowInstance = Instantiate(speedupArrowPrefab, player.transform.position + Vector3.up * speedupArrowsHeight, Quaternion.identity, player.transform);
        activeArrows[player] = arrowInstance;

        // Start arrow opacity animation
        Coroutine opacityCoroutine = StartCoroutine(LerpArrowOpacity(arrowInstance, BoostDuration));
        arrowCoroutines[player] = opacityCoroutine;

        // Wait for duration
        yield return new WaitForSeconds(BoostDuration);

        // Reset speed
        cmc.setSpeedMultiplier(1.0f);
        Debug.Log("Speed Boost Ended: Reset to 1.0");

        // Destroy visual effect
        if (effect) Destroy(effect);

        // Stop arrow opacity animation & destroy speedup arrows
        CleanupArrow(player);
    }

    private IEnumerator ApplySpeedBoost(NetworkCharacterMovementController ncmc, GameObject player)
    {
        if (ncmc == null) yield break;

        // Apply speed boost
        ncmc.setSpeedMultiplier(SpeedIncreaseMultiplier);
        Debug.Log("Speed Boost Applied (Network): " + SpeedIncreaseMultiplier);

        // Spawn visual effect
        GameObject effect = Instantiate(SpeedBoostEffectPrefab, ncmc.transform.position, Quaternion.identity);
        effect.transform.SetParent(ncmc.transform);

        // Spawn speedup arrow effect
        GameObject arrowInstance = Instantiate(speedupArrowPrefab, player.transform.position + Vector3.up * speedupArrowsHeight, Quaternion.identity, player.transform);
        activeArrows[player] = arrowInstance;

        // Start opacity animation
        Coroutine opacityCoroutine = StartCoroutine(LerpArrowOpacity(arrowInstance, BoostDuration));
        arrowCoroutines[player] = opacityCoroutine;

        // Wait for duration
        yield return new WaitForSeconds(BoostDuration);

        // Reset speed
        ncmc.setSpeedMultiplier(1.0f);
        Debug.Log("Speed Boost Ended (Network): Reset to 1.0");

        // Destroy visual effect
        if (effect) Destroy(effect);

        // Stop opacity animation & destroy speedup arrows
        CleanupArrow(player);
    }

    // Coroutine to lerp the opacity of the speedup arrow
    private IEnumerator LerpArrowOpacity(GameObject arrowObj, float duration)
    {
        if (arrowObj == null) yield break;

        SpriteRenderer[] arrowRenderers = arrowObj.GetComponentsInChildren<SpriteRenderer>();
        float arrowLerpTime = 0;
        bool increasing = true;
        float opacityLerpDuration = 0.5f;
        float elapsedTime = 0;

        while (arrowObj != null && elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Make arrow face each player's camera
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Camera playerCamera = player.GetComponentInChildren<Camera>();
                if (playerCamera != null)
                {
                    arrowObj.transform.LookAt(playerCamera.transform);
                    arrowObj.transform.Rotate(0, 180, 0);
                }
            }

            // Opacity pulsing effect
            if (increasing)
            {
                arrowLerpTime += Time.deltaTime / opacityLerpDuration;
                if (arrowLerpTime >= 1)
                {
                    arrowLerpTime = 1;
                    increasing = false;
                }
            }
            else
            {
                arrowLerpTime -= Time.deltaTime / opacityLerpDuration;
                if (arrowLerpTime <= 0)
                {
                    arrowLerpTime = 0;
                    increasing = true;
                }
            }

            float alpha = Mathf.Lerp(0, 1, arrowLerpTime);

            // Apply alpha to each arrow's SpriteRenderer
            foreach (SpriteRenderer renderer in arrowRenderers)
            {
                if (renderer == null) continue;
                Color newColor = renderer.color;
                newColor.a = alpha;
                renderer.color = newColor;
            }

            yield return null;
        }

        // Ensure arrow is fully transparent before being destroyed
        foreach (SpriteRenderer renderer in arrowRenderers)
        {
            if (renderer != null)
            {
                Color newColor = renderer.color;
                newColor.a = 0;
                renderer.color = newColor;
            }
        }
    }

    // Properly stops animation and destroys the arrow
    private void CleanupArrow(GameObject player)
    {
        if (activeArrows.ContainsKey(player))
        {
            GameObject arrow = activeArrows[player];

            // Stop the coroutine before destroying
            if (arrowCoroutines.ContainsKey(player))
            {
                StopCoroutine(arrowCoroutines[player]);
                arrowCoroutines.Remove(player);
            }

            // Ensure arrow is destroyed
            if (arrow)
            {
                Destroy(arrow);
            }

            activeArrows.Remove(player);
        }
    }
}
