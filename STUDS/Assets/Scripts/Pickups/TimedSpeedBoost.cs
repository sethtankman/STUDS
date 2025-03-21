using UnityEngine;

public class TimedSpeedBoost : MonoBehaviour
{
    private PowerupManager powerupManager;

    private void Start()
    {
        powerupManager = FindObjectOfType<PowerupManager>(); // Get the parent manager
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && powerupManager != null)
        {
            powerupManager.ActivateSpeedBoost(other.gameObject);
            Destroy(gameObject); // Destroy only the pickup, not the PowerupManager
        }
    }
}
