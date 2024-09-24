using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpSlowDown : MonoBehaviour
{
    [Range(0.0f, 2.0f)]
    public float SpeedAdjustment;

    private CharacterMovementController CMC;
    private NetworkCharacterMovementController NCMC;

    // Arrow texture and variables for opacity
    public GameObject slowdownArrowPrefab; // The arrow prefab to instantiate
    private List<GameObject> slowdownArrows = new List<GameObject>(); // List to store instantiated arrow GameObjects
    public float opacityLerpDuration = 1.0f; // Time for opacity lerp

    private float slowdownArrowsHeight = 2.25f; // How high to spawn the arrows

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NCMC = other.GetComponent<NetworkCharacterMovementController>();
            if (!NCMC)
            {
                CMC = other.GetComponent<CharacterMovementController>();
                CMC.setSpeedMultiplier(SpeedAdjustment);
            }
            else
            {
                NCMC.setSpeedMultiplier(SpeedAdjustment);
            }

            // Instantiate slowdown arrows above the player
            GameObject arrowInstance = Instantiate(slowdownArrowPrefab, other.transform.position + Vector3.up * slowdownArrowsHeight, Quaternion.identity, other.transform);
            slowdownArrows.Add(arrowInstance); // Add the arrow to the list

            // Start a new lerp coroutine for the specific arrow
            StartCoroutine(LerpArrowOpacity(arrowInstance));
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NCMC = other.GetComponent<NetworkCharacterMovementController>();
            if (!NCMC)
            {
                CMC = other.GetComponent<CharacterMovementController>();
                CMC.setSpeedMultiplier(1);
            }
            else
            {
                NCMC.setSpeedMultiplier(1);
            }

            // Destroy the arrows associated with the player
            DestroyArrowsForPlayer(other.transform);
        }
    }

    private void DestroyArrowsForPlayer(Transform playerTransform)
    {
        for (int i = slowdownArrows.Count - 1; i >= 0; i--)
        {
            if (slowdownArrows[i].transform.parent == playerTransform)
            {
                Destroy(slowdownArrows[i]);
                slowdownArrows.RemoveAt(i);
            }
        }
    }

    // Coroutine to lerp the opacity of the arrow
    private IEnumerator LerpArrowOpacity(GameObject arrowObj)
    {
        SpriteRenderer[] arrowRenderers = arrowObj.GetComponentsInChildren<SpriteRenderer>();
        float arrowLerpTime = 0;
        bool increasing = true;

        while (true)
        {
            // Update the rotation for all players' cameras
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                Camera playerCamera = player.GetComponentInChildren<Camera>();
                if (playerCamera != null)
                {
                    // Rotate arrow to face the player's camera
                    arrowObj.transform.LookAt(playerCamera.transform);
                    arrowObj.transform.Rotate(0, 180, 0); // Fix the orientation
                }
            }

            // Increment or decrement lerpTime based on direction
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

            // Lerp between 0 and 1 opacity
            float alpha = Mathf.Lerp(0, 1, arrowLerpTime);

            // Apply alpha to each arrow's SpriteRenderer
            foreach (SpriteRenderer renderer in arrowRenderers)
            {
                Color newColor = renderer.color;
                newColor.a = alpha;
                renderer.color = newColor;
            }

            yield return null;
        }
    }
}
