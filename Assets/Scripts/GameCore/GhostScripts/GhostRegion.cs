using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomExtensions;

public class GhostRegion : MonoBehaviour {

    public new string name;
    public float radius;

    public Color gizmoColour;

    public delegate void RegionEvent();

    void OnDrawGizmos()
    {
        Gizmos.color = gizmoColour;
        Gizmos.DrawWireSphere(transform.position.SetY(0), radius);
    }
}
