using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUpSlowDown : MonoBehaviour
{
    [Range(0.0f, 2.0f)]
    public float SpeedAdjustment;

    private float originalSpeedGrab;
    private float originalSpeedNormal;

    private CharacterMovementController CMC;
    private NetworkCharacterMovementController NCMC;

    public void OnTriggerEnter(Collider other)
    {
        // CompareTag("x") is more efficient than tag == "x"
        if (other.CompareTag("Player"))
        {         
            NCMC = other.GetComponent<NetworkCharacterMovementController>();
            if (!NCMC)
            {
                CMC = other.GetComponent<CharacterMovementController>();
                CMC.setMoveSpeed(CMC.getMoveSpeed() * SpeedAdjustment);
                CMC.CanJump = false;
                originalSpeedNormal = CMC.moveSpeedNormal;
                originalSpeedGrab = CMC.moveSpeedGrab;
            } else
            {
                NCMC.setMoveSpeed(NCMC.getMoveSpeed() * SpeedAdjustment);
                NCMC.CanJump = false;
                originalSpeedNormal = NCMC.moveSpeedNormal;
                originalSpeedGrab = NCMC.moveSpeedGrab;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NCMC = other.GetComponent<NetworkCharacterMovementController>();
            if (!NCMC)
            {
                CMC = other.GetComponent<CharacterMovementController>();
                CMC.setMoveSpeed(originalSpeedNormal);
                CMC.CanJump = true;
            } else
            {
                NCMC.setMoveSpeed(originalSpeedNormal);
                NCMC.CanJump = true;
            }
        }
    }
}
        

