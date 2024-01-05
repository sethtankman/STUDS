using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MoveToMulti : MonoBehaviour {

    public Transform[] Targets;         // User defined waypoints
    public bool visualizeTargets = false;
    private Transform[] targetList;     // Interal list of waypoints
    private int currentTarget = 1;
    private int prevTarget = 0;
    private int totalTargets = 0;
    [Tooltip("Meters per second")]
    public float Speed = 5f;
    private bool _reverse = false;
    public bool _pingPong = false;
    public bool _beginOnStart = false;

    public float speedMultiplier = 1;

    [Header("Type")]
    public bool translate = true;
    public bool rotate = false;

    private float lerpMask = 0;
    private bool _moving = false;

    private Vector3 startPos;
    private Quaternion startRot;

    [Header("Events")]
    public UnityEvent OnBegin;
    public UnityEvent OnStop;

    public UnityEvent OnReachEnd;
    public UnityEvent OnReachStart;

    private void Start()
    {
        ResetStartTransform();

        // Adding the start position to the list of targets
        totalTargets = Targets.Length + 1;
        targetList = new Transform[totalTargets];

        // We have to create an object to act as the transform of the object's start waypoint.
        GameObject startTransGO = new GameObject(gameObject.name + "-StartPosition");
        startTransGO.transform.parent = Targets[0].parent;  // Make sure it's in the same hierarchy as the other way points
        startTransGO.transform.position = startPos;
        startTransGO.transform.rotation = startRot;

        // Merging the arrays
        targetList.SetValue(startTransGO.transform, 0); 
        Targets.CopyTo(targetList, 1);

        if (_beginOnStart)
            Begin();
    }

    void Update ()
    {
        if (_moving)
            Move();
	}

    void Move()
    {
        float dist = Vector3.Distance(targetList[prevTarget].position, targetList[currentTarget].position);

        float rate = dist / (Speed * speedMultiplier);

        lerpMask += (Time.deltaTime / rate);

        if(translate)
            transform.position = Vector3.Lerp(targetList[prevTarget].position, targetList[currentTarget].position, lerpMask);
        if (rotate)
            transform.rotation = Quaternion.Slerp(targetList[prevTarget].rotation, targetList[currentTarget].rotation, lerpMask);

        //Debug.Log(lerpMask);
        if (lerpMask >= 1)
        {
            if ((currentTarget + 1) > totalTargets-1)
            {
                OnReachEnd.Invoke();
                CheckPingPong();
            }
            else if (currentTarget - 1 < 0)
            {
                OnReachStart.Invoke();
                CheckPingPong();
            }
            else
            {
                prevTarget = currentTarget;
                if (_reverse)
                    currentTarget--;
                else
                    currentTarget++;
                lerpMask = 0;
            }
        }
    }

    private void CheckPingPong()
    {
        if (_pingPong)
        {
            Reverse();
        }
        else
            Stop();
    }

    public void Begin()
    {
        _moving = true;
        OnBegin.Invoke();
    }

    public void Stop()
    {
        _moving = false;
        OnStop.Invoke();
    }

    public void Reverse()
    {
        int temp = prevTarget;
        prevTarget = currentTarget;
        currentTarget = temp;
        lerpMask = Mathf.Clamp(1 - lerpMask,0,1);
        _reverse = !_reverse;
    }

    public void ReverseAndMove()
    {
        _reverse = !_reverse;
        _moving = true;
    }

    public void ResetStartTransform()
    {
        startPos = transform.position;
        startRot = transform.rotation;
        lerpMask = 0;
    }

    private void OnDrawGizmos()
    {
        if (visualizeTargets && Targets.Length>0)
        {
            Debug.DrawLine(transform.position, Targets[0].position, Color.blue);
            for (int i = 0; i < Targets.Length - 1; i++)
            {
                Debug.DrawLine(Targets[i].position, Targets[i + 1].position, Color.blue);
            }
        }
    }

}
