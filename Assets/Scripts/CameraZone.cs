using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class CameraZone : MonoBehaviour {
    public CameraLimit cameraLimits = new CameraLimit();

    GameObject player;
    new GameObject camera;

    private void Start() {
        player = GameObject.Find("Player");
        camera = GameObject.Find("Main Camera");
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject == player) {
            camera.GetComponent<PlayerCameraController>().limits = cameraLimits;
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject == player) {
            if(camera.GetComponent<PlayerCameraController>().limits == cameraLimits)
                camera.GetComponent<PlayerCameraController>().limits = null;
        }
    }
}
