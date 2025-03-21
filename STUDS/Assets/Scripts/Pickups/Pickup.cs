using System.Collections;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float activationDelay = 3.0f; // Time before pickup becomes active
    public GameObject activationEffectPrefab; // Effect when pickup becomes active
    public GameObject collectionEffectPrefab; // Effect when collected

    private Collider pickupCollider;
    private Renderer pickupRenderer;
    [SerializeField] private Renderer meshRenderer;

    public Material DefaultMaterial;
    public Material OutlineMaterial;
    public float materialLerpDuration = 1.0f;

    [Header("Floating & Rotation")]
    public float floatHeight = 0.25f; // Height of floating motion
    public float floatSpeed = 2.0f; // Speed of floating motion
    public float rotationSpeed = 50.0f; // Rotation speed

    private Vector3 startPosition;

    private void Awake()
    {
        pickupCollider = GetComponent<Collider>();
        pickupRenderer = GetComponentInChildren<Renderer>(); // Assuming pickup has a visible mesh
        startPosition = transform.position;
    }

    private void Start()
    {
        if (pickupCollider) pickupCollider.enabled = false;
        if (pickupRenderer) pickupRenderer.enabled = false;

        StartCoroutine(ActivatePickupAfterDelay());
    }

    private void Update()
    {
        // Apply floating effect
        transform.position = startPosition + new Vector3(0, Mathf.Sin(Time.time * floatSpeed) * floatHeight, 0);
        // Apply rotation
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        
        // Continuously update material lerping
        PickupMaterialLerp();
    }

    private IEnumerator ActivatePickupAfterDelay()
    {
        yield return new WaitForSeconds(activationDelay);

        if (pickupCollider) pickupCollider.enabled = true;
        if (pickupRenderer) pickupRenderer.enabled = true;

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

            if (collectionEffectPrefab)
            {
                Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(transform.parent.gameObject);
        }
    }

    private void PickupMaterialLerp()
    {
        if (meshRenderer == null || DefaultMaterial == null || OutlineMaterial == null) return;

        float lerp = Mathf.PingPong(Time.time, materialLerpDuration) / materialLerpDuration;
        meshRenderer.material.Lerp(DefaultMaterial, OutlineMaterial, lerp);
    }
}
