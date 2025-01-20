using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LocalGrabbableObjectController : MonoBehaviour
{
    public float distance, height;
    public List<Collider> additionalColliders;
    public Vector3 rotation;
    public GameObject networkedGO;
    public GameObject throwableArrow;
    private Transform localT;
    private Vector3 offset;
    [SerializeField] private bool isLerp = false;
    private DateTime start;

    /// <summary>
    /// Calls LocalLetGo on NetworkedGO and sets throwableArrow to inactive.
    /// </summary>
    public void LocalLetGo()
    {
        if (throwableArrow)
        {
            throwableArrow.SetActive(false);
        }
        networkedGO.GetComponent<NetGrabbableObjectController>().LocalLetGo();
    }

    public void Update()
    {
        if(isLerp)
        {
            float t = (float)((DateTime.Now - start).TotalMilliseconds / 500);
            if(networkedGO)
            {
                Vector3 localProjection = offset + networkedGO.transform.position;
                transform.position = Vector3.Lerp(localProjection, networkedGO.transform.position, t);
                transform.rotation = Quaternion.Lerp(localT.rotation, networkedGO.transform.rotation, t);
                if (t > 1.0f)
                {
                    isLerp = false;
                    EndLerp();
                }
            } else
            {
                Destroy(gameObject);
            }
        }
    }

    private void EndLerp()
    {
        networkedGO.GetComponent<NetGrabbableObjectController>().RenderNetworkedGO();
        networkedGO.GetComponent<NetGrabbableObjectController>().SetCanPickup(true);
        Destroy(gameObject);
    }

    public void SetLerp(bool tf)
    {
        isLerp = tf;
        start = DateTime.Now;
        offset = localT.position - networkedGO.transform.position;
    }

    public void SetLocalT(Transform lt)
    {
        localT = lt;
    }
}
