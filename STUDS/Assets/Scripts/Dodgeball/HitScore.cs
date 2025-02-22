using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Scores points when a dodgeball hits a player.
/// For Online and Offline.
/// </summary>
public class HitScore : MonoBehaviour
{
    public int pointValue;

    /// <summary>
    /// Records the hit, plays the sound, and destroys the parent gameobject
    /// </summary>
    /// <param name="owner"></param>
    public void RecordHit(string owner)
    {
        if (owner.Length > 0)
        {
            if (DBGameManager.Instance)
                DBGameManager.Instance.AddPoints(owner, pointValue);
            else if (NetDBGameManager.Instance)
                NetDBGameManager.Instance.AddPoints(owner, pointValue);
            else
                Debug.LogError("No Game manager found!");
        }
        if(DBGameManager.Instance)
            Destroy(transform.parent.gameObject);
        else if (NetDBGameManager.Instance)
        {
            NetworkServer.Destroy(transform.parent.gameObject);
        }
    }
}
