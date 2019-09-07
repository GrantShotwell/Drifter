using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using MyStuff;
using MyStuff.GeometryObjects;

[RequireComponent(typeof(Light2D))]
public class FloodFill : MonoBehaviour {
    public float radius = 10.0f;
    public int density = 16;
    new Light2D light;

    private void OnValidate() {
        if(density < 1) density = 1;
    }

    private void Start() {
        light = GetComponent<Light2D>();
    }

    private void OnDrawGizmosSelected() {

        List<Vector3> hits = new List<Vector3>(density);
        Angle separation = Mathf.PI * 2 / density;
        
        int rays = density; Angle current = 0;
        while(rays --> 0) {
            var hit = Physics2D.Raycast(transform.position, Vector2.right.Rotate(current), radius);
            hits.Add(hit ? hit.point : Vector2.right.Rotate(current) * radius + (Vector2)transform.position);
            current += separation;
        }

        for(int i = 0; i < density; i++) {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, hits[i]);
        }

        for(int i = 1; i < density; i++) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(hits[i - 1], hits[i]);
        }   Gizmos.DrawLine(hits[0], hits[density - 1]);
    }

}
