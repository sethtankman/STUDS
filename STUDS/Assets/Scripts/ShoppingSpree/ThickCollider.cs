using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThickCollider : MonoBehaviour
{
    public Vector3 direction;

    private void OnCollisionStay(Collision collision)
    {
        collision.gameObject.GetComponent<Rigidbody>().position += direction;
    }
}
