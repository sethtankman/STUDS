using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitScore : MonoBehaviour
{
    public int pointValue;

    public void RecordHit(string owner)
    {
        if(owner.Length > 0)
            DBGameManager.Instance.AddPoints(owner, pointValue);
    }
}
