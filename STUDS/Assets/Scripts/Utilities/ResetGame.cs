using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    public void Reset()
    {
        if(ManagePlayerHub.Instance) {
            ManagePlayerHub.Instance.DeletePlayers();
            Destroy(ManagePlayerHub.Instance.gameObject);
        } else if (GameObject.Find("GameManager")) {
            GameObject gm = GameObject.Find("GameManager");
            gm.GetComponent<ManagePlayerHub>().DeletePlayers();
            Destroy(gm);
        }
    }
}
