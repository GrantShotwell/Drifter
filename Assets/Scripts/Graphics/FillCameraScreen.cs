using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCameraScreen : MonoBehaviour {
    public new Camera camera;

    private void Start() {
        if(camera == null) {
            camera = Camera.main;
        }
    }

    private void Update() {
        transform.position = (Vector2)camera.transform.position;
        transform.localScale = new Vector3(Screen.width * camera.orthographicSize / 200, Screen.height * camera.orthographicSize / 200, 1);
    }
}
