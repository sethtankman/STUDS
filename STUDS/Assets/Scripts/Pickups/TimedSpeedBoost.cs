using System.Collections;
using UnityEngine;

public class TimedSpeedBoost : MonoBehaviour
{
    [Range(0.0f, 2.0f)]
    public float SpeedIncreasePercentage = 1.2f; // 20% speed increase
    public float BoostDuration = 3.0f; // Boost duration in seconds

    public GameObject SpeedBoostEffectPrefab; // Assign in the Unity inspector

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CharacterMovementController CMC = other.GetComponent<CharacterMovementController>();
            NetworkCharacterMovementController NCMC = other.GetComponent<NetworkCharacterMovementController>();

            if (CMC)
            {
                StartCoroutine(ApplySpeedBoost(CMC));
            }
            else if (NCMC)
            {
                StartCoroutine(ApplySpeedBoost(NCMC));
            }

            // Destroy the pickup after activation
            Destroy(gameObject);
        }
    }

    private IEnumerator ApplySpeedBoost(CharacterMovementController cmc)
    {
        if (cmc == null) yield break;

        float originalSpeedMultiplier = 1.0f; // Default speed multiplier
        cmc.setSpeedMultiplier(originalSpeedMultiplier * SpeedIncreasePercentage);

        // Instantiate effect and attach it to the player
        GameObject effect = Instantiate(SpeedBoostEffectPrefab, cmc.transform.position, Quaternion.identity);
        effect.transform.SetParent(cmc.transform); // Ensure it follows the player

        yield return new WaitForSeconds(BoostDuration);

        // Reset speed multiplier
        cmc.setSpeedMultiplier(originalSpeedMultiplier);

        // Ensure effect is destroyed
        if (effect)
        {
            Destroy(effect);
        }
    }

    private IEnumerator ApplySpeedBoost(NetworkCharacterMovementController ncmc)
    {
        if (ncmc == null) yield break;

        float originalSpeedMultiplier = 1.0f;
        ncmc.setSpeedMultiplier(originalSpeedMultiplier * SpeedIncreasePercentage);

        // Instantiate effect and attach it to the player
        GameObject effect = Instantiate(SpeedBoostEffectPrefab, ncmc.transform.position, Quaternion.identity);
        effect.transform.SetParent(ncmc.transform); // Ensure it follows the player

        yield return new WaitForSeconds(BoostDuration);

        // Reset speed multiplier
        ncmc.setSpeedMultiplier(originalSpeedMultiplier);

        // Ensure effect is destroyed
        if (effect)
        {
            Destroy(effect);
        }
    }
}
