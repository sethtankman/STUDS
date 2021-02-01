using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoppingItem : MonoBehaviour
{
    private GameObject player;

    public string name;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayer(GameObject player) 
    {
        this.player = player;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
