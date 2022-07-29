using UnityEngine;
using Steamworks;

public class SteamworksVerification : MonoBehaviour
{
    private void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log(name);
        }
    }
}

