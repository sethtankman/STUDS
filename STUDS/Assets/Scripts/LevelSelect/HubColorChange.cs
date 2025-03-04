using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubColorChange : MonoBehaviour
{
    public Material playerColor;

    [SerializeField] private GameObject gameManager;

    public string colorName; // Set in editor

    public AK.Wwise.Event ParticleSound;


    private void OnTriggerEnter(Collider other)
    {
        gameManager = GameObject.Find("GameManager");
        if (other.CompareTag("Player"))
        {
            List<GameObject> players = gameManager.GetComponent<ManagePlayerHub>().getPlayers();

            bool isTaken = false;
            foreach (GameObject player in players)
            {
                if (player && player.GetComponent<CharacterMovementController>().GetColorName().Equals(colorName))
                {
                    isTaken = true;
                }
            }
            if (!isTaken)
            {
                ParticleSound.Post(gameObject);
                other.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().material = playerColor;
                other.GetComponent<CharacterMovementController>().SetColorName(colorName);
            }
        }
    }
}
