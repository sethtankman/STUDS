using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class SpawnEasterEgg : MonoBehaviour
{
    [SerializeField] private GameObject miniCar;
    private bool hasSpawned = false;

    private void OnTriggerStay(Collider other)
    {
        if(hasSpawned == false && other.GetComponent<CharacterMovementController>()
            && (Input.GetKey(KeyCode.E) && Input.GetKey(KeyCode.U) && Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.N))
            || (Input.GetKey(KeyCode.Joystick1Button1) && Input.GetKey(KeyCode.Joystick1Button2) && Input.GetKey(KeyCode.Joystick1Button3)))
        {
            GameObject car = Instantiate(miniCar, other.transform.position + new Vector3(0,0.5f,0), Quaternion.identity);
            car.transform.parent = other.transform;
            car.transform.forward = other.transform.forward * -1;
            other.GetComponent<CharacterMovementController>().moveSpeed = 16;
            Destroy(transform.parent.gameObject);
            hasSpawned = true;
        }
    }
}
