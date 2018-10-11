﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour {
    #region Variables
    public bool targetIsPlayer = true;
    [ConditionalField("targetIsPlayer", false)]
    public GameObject target;

    [Header("Zoom Options")]

    [DefinedValues("Fixed", "Speed")]
    public string zoom = "Fixed";
    public bool fxdZoom { get; private set; } = false;
    public bool spdZoom { get; private set; } = false;

    [System.Serializable]
    public class SpeedZoom {
        public float minSize = 1;
        public float baseSize = 5;
        public float maxSize = 10;
        public float multiplier = 1;
    }
    [ConditionalField("zoom", "Speed")]
    public SpeedZoom speedZoom = new SpeedZoom();
    
    [Header("Follow Options")]
    
    public bool smooth = false;

    [ConditionalField("smooth")]
    public float lerpAmount = 0.1f;
    #endregion

    #region Update
    private void Start() {
        if(targetIsPlayer) target = GameObject.Find("Player");
        switch(zoom) {
            case "Fixed":
                fxdZoom = true;
                break;
            case "Speed":
                spdZoom = true;
                break;
        } //start with strings for usability, end with bools for preformance
        zoom = null;
    }

    private void Update() {
        #region Position Update
        if(smooth) {
            Vector2 position2D = Vector2.Lerp(
                transform.position,
                target.transform.position,
                lerpAmount * Time.deltaTime
            );
            transform.position = (Vector3)position2D
                + new Vector3(0, 0, transform.position.z);
        }
        else {
            transform.position = new Vector3(
                target.transform.position.x,
                target.transform.position.y,
                transform.position.z
            );
        }
        #endregion

        #region Zoom Update
        if(spdZoom) {
            VelocityTracker speedTracker = target.GetComponent<VelocityTracker>();
            if(speedTracker != null) {
                GetComponent<Camera>().orthographicSize = speedTracker.averageSpeed * speedZoom.multiplier + speedZoom.baseSize;
                if(GetComponent<Camera>().orthographicSize > speedZoom.maxSize) GetComponent<Camera>().orthographicSize = speedZoom.maxSize;
                if(GetComponent<Camera>().orthographicSize < speedZoom.minSize) GetComponent<Camera>().orthographicSize = speedZoom.minSize;
            }
            else Debug.LogWarning("The target doesn't have a 'VelocityTracker' component!");
        }
        #endregion
    }
    #endregion
}
