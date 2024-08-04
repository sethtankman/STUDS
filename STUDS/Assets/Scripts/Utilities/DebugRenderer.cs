using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugRenderer : MonoBehaviour
{
    [SerializeField] private bool debugging;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().enabled = debugging;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
