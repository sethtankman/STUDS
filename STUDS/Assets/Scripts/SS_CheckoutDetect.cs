using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_CheckoutDetect : MonoBehaviour
{
    public ParticleSystem CheckoutEffect;
    public AK.Wwise.Event RegisterSound;

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
            RegisterSound.Post(gameObject);
            GameObject player = collision.collider.gameObject.GetComponent<ShoppingItem>().GetPlayer();
            string itemName = collision.collider.gameObject.GetComponent<ShoppingItem>().name;
            string[] itemlist = player.GetComponent<SS_ItemTracker>().GetList();
            for(int i = 0; i < itemlist.Length; i++)
            {
                if (itemlist[i].Equals(itemName) && player.GetComponent<SS_ItemTracker>().isItemCompleted(itemName) == false)
                {
                    player.GetComponent<SS_ItemTracker>().CheckoutItem(i);
                    CheckoutEffect.Play();
                    Destroy(collision.collider.gameObject);
                }
            }
        }
    }
}
