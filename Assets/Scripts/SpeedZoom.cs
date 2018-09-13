using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedZoom : MonoBehaviour {
    public float baseSize = 5;
    public float multiplier = 1;

    GameObject target;
    VelocityTracker velocityTracker;
    bool disabled = false;

    void Start() {
        target = GameObject.Find("Player");
        velocityTracker = target.GetComponent<VelocityTracker>();
    }

    void LateUpdate() {
        if(!disabled) gameObject.GetComponent<Camera>().orthographicSize = velocityTracker.averageSpeed * multiplier + baseSize;
    }

    void ChangeTarget(GameObject gObj) {
        target = gObj;
        velocityTracker = target.GetComponent<VelocityTracker>();
    }

    void Disable() { disabled = true; }
    void Enable() { disabled = false; }
}
