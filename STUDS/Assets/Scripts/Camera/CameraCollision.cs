using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This code references Filmstorm's video 
/// "Free 3rd Person Camera Setup & Camera Collision Tutorial" on YouTube
/// </summary>
public class CameraCollision : MonoBehaviour
{
    public float minDistance = 4.0f;
    public float maxDistance = 10.0f;
    public float smooth = 10.0f;
    Vector3 dollyDirection;
    public Vector3 dollyDirAdjusted;
    public float distance;

    // Start is called before the first frame update
    void Awake()
    {
        dollyDirection = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPosition = transform.parent.TransformPoint(dollyDirection * maxDistance);
        RaycastHit hit;
        // The mesh renderer check is quick fix to stop collisions with invisible objects.
        if (Physics.Linecast(transform.parent.position, desiredCameraPosition, out hit) && hit.transform.gameObject.layer != 2)// && hit.transform.GetComponent<MeshRenderer>()) 
        {
            //Debug.Log("Camera collided with " + hit.transform.gameObject.name);
            // Debug.Log("Distance to hit: " + hit.distance);
            // Debug.Log("Dolly Direction: " + dollyDirection);
            distance = Mathf.Clamp((hit.distance * 0.9f), minDistance, maxDistance);
        } else
        {
            distance = maxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDirection * distance, Time.deltaTime * smooth);
    }
}
