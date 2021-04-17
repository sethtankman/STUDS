using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Reset()
    {
        GameObject gameManager = GameObject.Find("GameManager");
        gameManager.GetComponent<ManagePlayerHub>().DeletePlayers();
        Destroy(gameManager);
    }
}
