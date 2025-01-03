﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    //declarations
    private Transform startPoint;
    public Transform[] Waypoints;
    public bool warpToStart = false;
    public float speed;
    public float halt;


    //These are used to determine which next step to take
    private float percentageMoved;
    private float startTime;
    private Transform pointFrom;
    private Transform pointTo;
    private int waypointIndex = 0;
    private float distance;
    private bool moving = false;


    // Start is called before the first frame update
    void Start()
    {
        
        startPoint = transform;
        percentageMoved = 0.0f;
        setPoints();
        
    }

    // Update is called once per frame
    void Update()
    {
        //Calculates the percentage moved between points       
        float timeSinceBegan =  (Time.time - startTime) * speed;
        percentageMoved = timeSinceBegan / distance;

        //Interpolates between the points
        if (moving)
        {
            transform.position = Vector3.Lerp(pointFrom.position, pointTo.position, percentageMoved);
            transform.rotation = Quaternion.Lerp(pointFrom.rotation, pointTo.rotation, percentageMoved);
        }

        if (percentageMoved >= 1.0f && waypointIndex + 1 < Waypoints.Length && moving)
        {
            waypointIndex++;
            setPoints();
        }

        if(waypointIndex + 1 >= Waypoints.Length && moving)
        {
            if (warpToStart)
            {
                waypointIndex = 0;
                setPoints();
            }
            else
            {
                MovetoStart();
                waypointIndex = -1;
            }
        }                    
    }

    //This function sets the next point to interpolate to
    void setPoints()
    {
        pointFrom = Waypoints[waypointIndex];
        if(waypointIndex+1 < Waypoints.Length)
            pointTo = Waypoints[waypointIndex + 1];
        StartCoroutine(resetTime());
        distance = Vector3.Distance(pointFrom.position, pointTo.position);
    }

    void MovetoStart()
    {
        pointFrom = Waypoints[waypointIndex];
        pointTo = Waypoints[0];
        StartCoroutine(resetTime());
        distance = Vector3.Distance(pointFrom.position, pointTo.position);
    }

    IEnumerator resetTime()
    {
        moving = false;
        yield return new WaitForSeconds(halt);
        startTime = Time.time;
        moving = true;
        yield return null;
    }

}
