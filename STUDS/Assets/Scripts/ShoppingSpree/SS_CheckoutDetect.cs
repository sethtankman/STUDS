using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SS_CheckoutDetect : MonoBehaviour
{
    public ParticleSystem CheckoutEffect;
    public AK.Wwise.Event RegisterSound;
    public SteamAchievements sa;

    public GameObject timerManager;

    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ShoppingItem"))
        {
            GameObject player = collision.collider.gameObject.GetComponent<ShoppingItem>().GetPlayer();
            string itemName = collision.collider.gameObject.GetComponent<ShoppingItem>().name;
            string[] itemlist = player.GetComponent<SS_ItemTracker>().GetList();
            for (int i = 0; i < itemlist.Length; i++)
            {
                if (itemlist[i].Equals(itemName) && player.GetComponent<SS_ItemTracker>().isItemCompleted(itemName) == false)
                {
                    RegisterSound.Post(gameObject);
                    player.GetComponent<SS_ItemTracker>().CheckoutItem(i);
                    CheckoutEffect.Play();
                    Destroy(collision.collider.gameObject);
                    //sa.UnlockAchievement("SS_CHECKOUT");
                }
            }

            bool allDone = true;
            foreach (string item in itemlist)
            {
                if (!player.GetComponent<SS_ItemTracker>().isItemCompleted(item))
                {
                    allDone = false;
                }
            }

            if (allDone && player.GetComponent<CharacterMovementController>() && player.GetComponent<CharacterMovementController>().GetFinishPosition() == 0)
            {
                player.GetComponent<CharacterMovementController>().SetFinishPosition(timerManager.GetComponent<ShoppingTimer>().racePositions);
                timerManager.GetComponent<ShoppingTimer>().racePositions++;
            }
        }
    }
}
