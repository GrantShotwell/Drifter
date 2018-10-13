using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class WatcherController : MonoBehaviour {
    #region Variables
    public GameObject target;

    [System.Serializable]
    public class Eye {
        [Header("GameObjects")]
        public GameObject parent;
        public GameObject light;
        public GameObject iris;
        public GameObject irisLight;
        public GameObject lid0, lid1;

        [Header("Iris Settings")]
        public float irisSpeed;
        public float radius;

        [Header("Lid Settings")]
        public Vector2 lidClosed;
        public Vector2 lidOpened;
        public float lidSpeed;
        public int lidState;

        [HideInInspector]
        public Vector2
            irisTarget = new Vector2(),
            lid0Target = new Vector2(),
            lid1Target = new Vector2();
    }
    public Eye eye = new Eye();

    Hover hoverScript;
    TimedActionTracker timedActions = new TimedActionTracker();
    #endregion
    private void Start() {
        hoverScript = GetComponent<Hover>();
        hoverScript.enabled = false;
    }
    
    private void Update() {
        timedActions.Update();

        #region Eye
        eye.light.SetActive(eye.lidState == 1);
        eye.irisLight.SetActive(eye.lidState == 1);

        #region Lids
        switch(eye.lidState) {
            case 0: //closed
                eye.lid0Target =  eye.lidClosed;
                eye.lid1Target = -eye.lidClosed;
                break;
            case 1: //open
                eye.lid0Target =  eye.lidOpened;
                eye.lid1Target = -eye.lidOpened;
                break;
        }
        
        eye.lid0.transform.position = Vector2.Lerp(eye.lid0.transform.position, eye.lid0Target.Rotate(eye.parent.transform.rotation.eulerAngles.z) + (Vector2)eye.parent.transform.position, eye.lidSpeed);
        eye.lid1.transform.position = Vector2.Lerp(eye.lid1.transform.position, eye.lid1Target.Rotate(eye.parent.transform.rotation.eulerAngles.z) + (Vector2)eye.parent.transform.position, eye.lidSpeed);
        #endregion

        #region Iris
        eye.irisTarget = target.transform.position;

        float angleToTarget = Geometry.GetAngle(eye.irisTarget, eye.iris.transform.position);
        eye.iris.transform.position += (Vector3)(Vector2.right.Rotate(angleToTarget) * eye.irisSpeed * Time.deltaTime);
        float angleToEye = Geometry.GetAngle(eye.parent.transform.position, eye.iris.transform.position);
        if(eye.irisLight.activeSelf)
            eye.irisLight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleToEye + 90));

        float distanceFromEye = Vector2.Distance(eye.parent.transform.position, eye.iris.transform.position);
        if(distanceFromEye > eye.radius) {
            float distanceOverRadius = distanceFromEye - eye.radius;
            eye.iris.transform.position += (Vector3)(Vector2.right.Rotate(angleToEye) * distanceOverRadius);
        }
        #endregion
        #endregion
    }

    public void Activate() {
        eye.lidState = 1;
        timedActions.AddAction(new TimedAction(2, () => {
            hoverScript.enabled = true;
        }));
        timedActions.AddAction(new ContinuousAction(5, 7, () => {
            float angle = transform.rotation.eulerAngles.z;
            transform.rotation = Quaternion.Euler(0, 0, angle - (45 * Time.deltaTime));
            if(180 < transform.rotation.eulerAngles.z) transform.rotation = Quaternion.Euler(0, 0, 0);
        }));
        timedActions.AddAction(new TimedAction(7, () => {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }));
    }
}
