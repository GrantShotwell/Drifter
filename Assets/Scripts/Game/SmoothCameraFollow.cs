using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour {
    public float lerpAmount = 0.1f;
    GameObject target;
    
    void Start() {
        target = GameObject.Find("Player");
    }

    void Update() {
        float z = transform.position.z;
        Vector2 newPosition = target.transform.position;
        transform.position = new Vector3(newPosition.x, newPosition.y, z);
    }
}
