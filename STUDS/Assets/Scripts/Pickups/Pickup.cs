using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float activationDelay = 3.0f; // Time before pickup becomes active
    public GameObject activationEffectPrefab; // Effect when pickup becomes active
    public GameObject collectionEffectPrefab; // Effect when collected

    private Collider pickupCollider;
    private Renderer pickupRenderer;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        pickupRenderer = GetComponentInChildren<Renderer>(); // Assuming pickup has a visible mesh
    }

    private void Start()
    {
        // Disable interaction but keep object alive
        if (pickupCollider) pickupCollider.enabled = false;
        if (pickupRenderer) pickupRenderer.enabled = false;

        StartCoroutine(ActivatePickupAfterDelay());
    }

    private IEnumerator ActivatePickupAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        // Enable pickup visibility and collider
        if (pickupCollider) pickupCollider.enabled = true;
        if (pickupRenderer) pickupRenderer.enabled = true;

        // Instantiate activation effect
        if (activationEffectPrefab)
        {
            Instantiate(activationEffectPrefab, transform.position, Quaternion.identity);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PowerupManager powerupManager = FindObjectOfType<PowerupManager>();
            if (powerupManager != null)
            {
                powerupManager.ActivateSpeedBoost(other.gameObject);
            }

            // Instantiate collection effect
            if (collectionEffectPrefab)
            {
                Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); // Destroy pickup after collection
        }
    }
}
