using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class SpawnEasterEgg : MonoBehaviour
{
    [SerializeField] private GameObject miniCar;
    private bool hasSpawned = false;

    private void OnTriggerStay(Collider other)
    {
        if(hasSpawned == false && other.GetComponent<CharacterMovementController>() && other.GetComponent<CharacterMovementController>().isMini 
            && Input.GetKeyDown(KeyCode.E) && Input.GetKeyDown(KeyCode.U) && Input.GetKeyDown(KeyCode.G) && Input.GetKeyDown(KeyCode.I) && Input.GetKeyDown(KeyCode.N))
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
