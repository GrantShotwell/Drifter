using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform target;

    public enum FollowType { Lerp, Velocity, Position };
    public FollowType followType;
    Rigidbody2D targetRigidbody;

    public float lerp = 0.1f;
    public float velMult = 0.5f;

    private void Start() {
        targetRigidbody = target.GetComponent<Rigidbody2D>();
        if(followType == FollowType.Velocity && targetRigidbody == null)
            followType = FollowType.Position;
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
                    transform.position = new Vector3(end.x, end.y, cam.z);
                    break;
                case FollowType.Position:
                    transform.position = new Vector3(pos.x, pos.y, cam.z);
                    break;
                case FollowType.Velocity:
                    vel = targetRigidbody.velocity * velMult + (Vector2)target.transform.position;
                    end = Vector2.Lerp(cam, vel, lerp);
                    transform.position = new Vector3(end.x, end.y, cam.z);
                    break;
                default: break;
            }
            #endregion
        }
    }
}
