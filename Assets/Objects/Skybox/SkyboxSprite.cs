using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class SkyboxSprite : MonoBehaviour {
    private struct CameraInfo {
        public Camera camera;
        public Transform transform;
        public GameObject gameObject;

        public CameraInfo(Camera camera) {
            this.camera = camera;
            gameObject = camera.gameObject;
            transform = gameObject.transform;
        }
    }

    private CameraInfo cam;

    private void Start() {
        Camera camera = MyFunctions.AddCameraListener(this);
        cam = new CameraInfo(camera);
    }

    private void OnCameraReady() {

        transform.position = new Vector3(
            cam.transform.position.x,
            cam.transform.position.y,
            transform.position.z
        );

        transform.localScale = new Vector3(
            cam.camera.orthographicSize,
            cam.camera.orthographicSize,
            0
        );

    }

}
