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
    private Transform localT;
    private Vector3 offset;
    private bool isLerp = false;
    private DateTime start;

    public void LocalLetGo()
    {
        networkedGO.GetComponent<NetGrabbableObjectController>().LocalLetGo();
    }

    public void Update()
    {
        if(isLerp)
        {
            float t = (float)((DateTime.Now - start).TotalMilliseconds / 500);
            Vector3 localProjection = offset + networkedGO.transform.position; 
            transform.position = Vector3.Lerp(localProjection, networkedGO.transform.position, t);
            Debug.Log($"t = {t}, Position = {transform.position - localProjection}");
            transform.rotation = Quaternion.Lerp(localT.rotation, networkedGO.transform.rotation, t);
            if (t > 1.0f)
            {
                isLerp = false;
                EndLerp();
            }
        }
    }

    private void EndLerp()
    {
        networkedGO.GetComponent<NetGrabbableObjectController>().RenderNetworkedGO();
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
