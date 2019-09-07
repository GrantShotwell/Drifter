using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyStuff;

/// <summary>
/// Abstract class for cutscene control.
/// </summary>
public abstract class Cutscene : MonoBehaviour {
    [System.Serializable]
    public class Events {
        public UnityEvent onActive;
        public UnityEvent onFinish;
    }

    [Header("Basic Properties")]

    /// <summary>Is the cutscene currently running? - [WARNING] It is not recommended to set this variable. Use Activate() and Deactivate() instead.</summary>
    public bool active = false; //make read only

    /// <summary>The player GameObject.</summary>
    public PlayerClass player;

    /// <summary>Disable player input while active?</summary>
    public bool stopPlayer = true;

    /// <summary>Limit the player's x position?</summary>
    public bool limitX = false;

    /// <summary>What is the player's x position limited to?</summary>
    public Range xRange; //show only if limitX==true

    /// <summary>Finish() { Destroy(gameObject); } </summary>
    public bool destroyOnFinish = true;

    /// <summary>TimedActionTracker for this cutscene.</summary>
    [HideInInspector] public TimedActionTracker timedActions = new TimedActionTracker();

    /// <summary>The camera limits from when the cutscene started.</summary>
    [HideInInspector] public CameraLimit originalLimits;

    /// <summary>The CameraController of the main camera.</summary>
    [HideInInspector] public CameraController cameraController => Camera.main.GetComponent<CameraController>();
    
    [Header("Other Properties")]

    /// <summary>UnityEvents run from Begin() and Finish().</summary>
    public Events events = new Events();

    private void OnValidate() {
        if(player == null) player = GameObject.Find("Player").GetComponent<PlayerClass>();
    }

    private void Update() {
        timedActions.Update();
        if(active) {
            if(limitX) {
                var ppos = player.transform.position;
                float newX = xRange.Place(ppos.x);
                player.transform.position = new Vector3(newX, ppos.y, ppos.z);
            }
            During();
        }
    }

    /// <summary>Activate this cutscene.</summary>
    /// <param name="force">if(active == false || force) Activate();</param>
    public void Activate(bool force = false) {
        if(active == false || force) {
            active = true;
            //originalLimits = Camera.main.GetComponent<CameraController>().limits;
            if(stopPlayer) player.controller.enabled = false;
            Begin();
            events.onActive.Invoke();
        }
    }

    /// <summary>Deactivate this cutscene.</summary>
    /// <param name="force">if(active == true || force) Deactiveate();</param>
    public void Deactivate(bool force = false) {
        if(active == true || force) {
            active = false;
            if(stopPlayer) player.controller.enabled = true;
            Finish();
            events.onFinish.Invoke();
            if(destroyOnFinish) Destroy(gameObject);
        }
    }

    /// <summary>
    /// Method called on Activate().
    /// </summary>
    public abstract void Begin();

    /// <summary>
    /// Method called every Update() while active.
    /// </summary>
    public abstract void During();

    /// <summary>
    /// Method called on Deactivate().
    /// </summary>
    public abstract void Finish();
}
