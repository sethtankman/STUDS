using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScore : MonoBehaviour
{
    public int pointValue;

    /// <summary>
    /// Records the hit and destroys the parent gameobject
    /// </summary>
    /// <param name="owner"></param>
    public void RecordHit(string owner)
    {
        if(owner.Length > 0)
            DBGameManager.Instance.AddPoints(owner, pointValue);
        Destroy(transform.parent.gameObject);
    }
}
