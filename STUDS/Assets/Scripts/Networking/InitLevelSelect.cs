using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitLevelSelect : MonoBehaviour
{
    void Start()
    {
        PauseV2.canPause = true;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<CharacterMovementController>().CanMove = true;
        }
    }
}
