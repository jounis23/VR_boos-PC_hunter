using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
    public float radius;
    public Color color;

    void Start()
    {
        
    }

    // Update is ca
    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(this.transform.position, radius);
        Gizmos.color = color;
    }
}
