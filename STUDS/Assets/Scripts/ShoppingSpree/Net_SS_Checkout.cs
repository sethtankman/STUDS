using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Net_SS_Checkout : NetworkBehaviour
{
    public ParticleSystem CheckoutEffect;
    public AK.Wwise.Event RegisterSound;
    public SteamAchievements sa;

    public NetShopTimer timerManager;

    // Start is called before the first frame update
    void Start()
    {
        sa = GameObject.Find("SteamScripts").GetComponent<SteamAchievements>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("ShoppingItem"))
        {
            GameObject player = collision.gameObject.GetComponent<ShoppingItem>().GetPlayer();
            string itemName = collision.gameObject.GetComponent<ShoppingItem>().name;
            string[] itemlist = player.GetComponent<SS_ItemTracker>().GetList();
            for (int i = 0; i < itemlist.Length; i++)
            {
                if (itemlist[i].Equals(itemName) && player.GetComponent<SS_ItemTracker>().isItemCompleted(itemName) == false)
                {
                    RegisterSound.Post(gameObject);
                    player.GetComponent<SS_ItemTracker>().CheckoutItem(i);
                    CheckoutEffect.Play();
                    if (player.GetComponent<NetworkCharacterMovementController>().isLocalPlayer)
                        player.GetComponent<NetworkCharacterMovementController>().CmdDestroyObject(collision.gameObject.GetComponent<NetworkIdentity>().netId);
                    sa.UnlockAchievement("SS_CHECKOUT");
                }
            }
            if (isServer) {
                bool allDone = true;
                foreach (string item in itemlist)
                {
                    if (!player.GetComponent<SS_ItemTracker>().isItemCompleted(item))
                    {
                        allDone = false;
                    }
                }
                if (allDone && player.GetComponent<NetworkCharacterMovementController>() && player.GetComponent<NetworkCharacterMovementController>().GetFinishPosition() == 0)
                {
                    player.GetComponent<NetworkCharacterMovementController>().SetFinishPosition(timerManager.racePositions);
                    timerManager.IncrementRacePositions();
                }
            }
        }
    }

}
