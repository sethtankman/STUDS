using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalGrabbableObjectController : MonoBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    public GameObject networkedGO;

    public void LocalLetGo()
    {
        networkedGO.GetComponent<NetGrabbableObjectController>().LocalLetGo();
    }

}
