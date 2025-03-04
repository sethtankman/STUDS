using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetHubColorChange : NetworkBehaviour
{
    public Material playerColor;

    [SerializeField] private GameObject gameManager;

    public string colorName; // Set in editor

    public AK.Wwise.Event ParticleSound;


    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            gameManager = GameObject.Find("NetGameManager");
            if (other.CompareTag("Player"))
            {
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

                bool isTaken = false;
                foreach (GameObject player in players)
                {
                    if (player.GetComponent<NetworkCharacterMovementController>().GetColorName().ToLower().Equals(colorName.ToLower()))
                    {
                        isTaken = true;
                    }
                }
                if (!isTaken)
                {
                    ChangePlayerColor(other.gameObject);
                }
            }
        }
    }

    [ClientRpc]
    private void ChangePlayerColor(GameObject other)
    {
        ParticleSound.Post(gameObject);
        other.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor;
        other.GetComponent<NetworkCharacterMovementController>().SetColorName(colorName);
    }
}
