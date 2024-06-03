using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


// Copied from: https://www.youtube.com/watch?v=nrRfqS6u_zg
[RequireComponent(typeof(LineRenderer))]
public class NavigationDebugger : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agentToDebug;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(agentToDebug)
            if (agentToDebug.hasPath)
            {
                lr.positionCount = agentToDebug.path.corners.Length;
                lr.SetPositions(agentToDebug.path.corners);
                lr.enabled = true;
            } else
            {
                lr.enabled = false;
            }
    }
}
