using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowGizmo : MonoBehaviour {

    public Color color = Color.white;
    public float size = 1f;
    public bool hideGizmo = false;

    void OnDrawGizmos()
    {
        if (!hideGizmo)
        {
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position, size);
        }
    }
}
