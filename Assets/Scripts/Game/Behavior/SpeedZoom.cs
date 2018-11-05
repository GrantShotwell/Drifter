using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedZoom : MonoBehaviour {
    public float baseSize;
    public float multiplier;
    public float maxSize;

    GameObject target;
    VelocityTracker velocityTracker;
    bool disabled = false;

    void Start() {
        target = GameObject.Find("Player");
        velocityTracker = target.GetComponent<VelocityTracker>();
    }

    void LateUpdate() {
        if(!disabled) GetComponent<Camera>().orthographicSize = velocityTracker.averageSpeed * multiplier + baseSize;
        if(GetComponent<Camera>().orthographicSize > maxSize) GetComponent<Camera>().orthographicSize = maxSize;
    }

    void ChangeTarget(GameObject gObj) {
        target = gObj;
        velocityTracker = target.GetComponent<VelocityTracker>();
    }

    void Disable() { disabled = true; }
    void Enable() { disabled = false; }
}
