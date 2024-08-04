using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Shows a trajectory for a throwable object
/// Reference: https://www.youtube.com/watch?v=p8e4Kpl9b28 
/// https://www.youtube.com/watch?v=U3hovyIWBLk
/// </summary>
public class ThrowSpline : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField, Min(3)] 
    private int maxIterations = 20;
    [SerializeField, Min(1)]
    private float timeOfFlight = 5;
    [SerializeField] private Vector3 throwForce = Vector3.zero;
    private float mass;

    private void Start()
    {
        mass = transform.parent.GetComponent<Rigidbody>().mass;
    }

    private void Update()
    {
        ShowTrajectoryLine();   
    }

    public void ShowTrajectoryLine()
    {
        float timeStep = timeOfFlight / maxIterations;
        lineRenderer.positionCount = maxIterations;
        lineRenderer.SetPositions(CalculateTrajectoryLine(timeStep));
    }

    private Vector3[] CalculateTrajectoryLine(float _timeStep)
    {
        Vector3[] lineRendererPoints = new Vector3[maxIterations];

        lineRendererPoints[0] = transform.localPosition;

        for(int i=1; i< maxIterations; i++)
        {
            float timeOffset = _timeStep * i;
            Vector3 progressBeforeGravity = transform.InverseTransformVector(throwForce) / mass * Time.fixedDeltaTime * timeOffset;
            Vector3 gravityOffset = Vector3.up * -0.5f * Physics.gravity.y * timeOffset * timeOffset;
            Vector3 newPosition = transform.localPosition + progressBeforeGravity - gravityOffset;
            lineRendererPoints[i] = newPosition;

        }
        return lineRendererPoints;
    }

    public void SetThrowForce(Vector3 _throwForce)
    {
        throwForce = _throwForce;
    }
}
