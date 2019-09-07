using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.LWRP;
using MyStuff;
using MyStuff.GeometryObjects;

public class CameraController : MonoBehaviour {
    public enum FollowType { Lerp, Velocity, Position };

    public Transform target;
    public FollowType followType;
    public float lerp = 0.1f;
    public float velMult = 0.5f;
    public float baseSize = 9f;
    public float zoomMultiplier = 3.0f;
    public int velocityPoints = 100;

    Rigidbody2D targetRigidbody;
    new Camera camera;
    RollingFloatArray velocities;

    public Camera GetCamera => camera ?? GetComponent<Camera>();

    [HideInInspector]
    public List<MonoBehaviour> recievers = new List<MonoBehaviour>(8);

    private void Start() {
        if(target != null)
            MoveToPosition(target.transform.position);
        velocities = new RollingFloatArray(velocityPoints);
        targetRigidbody = target.GetComponent<Rigidbody2D>();
        if(followType == FollowType.Velocity && targetRigidbody == null)
            followType = FollowType.Position;
        camera = GetComponent<Camera>();
        camera.orthographicSize = baseSize;
    }

    private void LateUpdate() {
        if(target != null) {

            #region Position
            //- Determines the position of the camera based on 'followType' and 'target'. -//

            //Camera position, target position/velocity, and final position of the camera.
            Vector3 cam, pos;
            Vector2 vel, end;
            cam = transform.position;
            pos = target.transform.position;

            //Which option was picked via 'followType'?
            switch(followType) {
                case FollowType.Lerp:
                    end = Vector2.Lerp(cam, pos, lerp);
                    MoveToPosition(end);
                    break;
                case FollowType.Position:
                    end = pos;
                    MoveToPosition(end);
                    break;
                case FollowType.Velocity:
                    vel = targetRigidbody.velocity * velMult + (Vector2)target.transform.position;
                    end = Vector2.Lerp(cam, vel, lerp);
                    MoveToPosition(end);
                    break;
                default: break;
            }

            #endregion

        }

        if(targetRigidbody != null)
            velocities.Add(targetRigidbody.velocity.magnitude);
        camera.orthographicSize = baseSize + velocities.average * zoomMultiplier;

        foreach(MonoBehaviour component in recievers)
            component.SendMessage("OnCameraReady", SendMessageOptions.DontRequireReceiver);
    }

    private void MoveToPosition(Vector2 position) {
        transform.position = new Vector3(
            position.x,
            position.y,
            transform.position.z
        );
    }
}
