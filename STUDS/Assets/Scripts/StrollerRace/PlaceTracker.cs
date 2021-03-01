using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceTracker : MonoBehaviour
{
    public List<GameObject> ProgressPoints = new List<GameObject>();
    public int Progress = 0;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "RacePoint")
        {
            if (ProgressPoints.Contains(other.gameObject))
            {
                if (Progress > 0)
                {
                    Progress -= 1;
                    ProgressPoints.Remove(other.gameObject);
                }
                else Progress = 0;
            }
            else
            {
                Progress += 1;
                ProgressPoints.Add(other.gameObject);

            }
        }
    }

 

}
