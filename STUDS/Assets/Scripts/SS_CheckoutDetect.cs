using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_CheckoutDetect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "ShoppingItem")
        {
            GameObject player = collision.collider.gameObject.GetComponent<ShoppingItem>().GetPlayer();
            string itemName = collision.collider.gameObject.GetComponent<ShoppingItem>().name;
            string[] itemlist = player.GetComponent<SS_ItemTracker>().GetList();
            for(int i = 0; i < itemlist.Length; i++)
            {
                if (itemlist[i].Equals(itemName))
                {
                    player.GetComponent<SS_ItemTracker>().CheckoutItem(i);
                }
            }
            Destroy(collision.collider.gameObject);
        }
    }
}
