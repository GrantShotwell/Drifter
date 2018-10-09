using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimGuidence : MonoBehaviour {
    public Material material;
    public Color color;
    GameObject player;
    new Camera camera;
    bool hidden = false;
    Vector2 mousePos, playerPos, camPos;
    Vector2 screenDimentions = new Vector2(Screen.width, Screen.height);
    Vector2 hitpoint, start, end;
    RaycastHit2D hit;

    void Start () {
        player = GameObject.Find("Player");
        camera = Camera.main;
	}

    void LateUpdate() {
        if(!hidden) {
            mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
            playerPos = player.transform.position;
            camPos = transform.position;
            hit = Physics2D.Raycast(playerPos, mousePos - camPos, Mathf.Infinity, player.GetComponent<PlayerController>().ropeLayermask);
            hitpoint = hit.point;
            Line rayLine = Geometry.LineFromTwoPoints(playerPos, mousePos);
            start = new Vector2(0, 0);
            end = new Vector2(0, 0);
            if(!rayLine.isVertical) {
                start = playerPos;
                if(hit.collider != null) end = hitpoint;
                else end = rayLine.PointFromDistance(
                    playerPos,
                    Vector2.Distance(
                        playerPos,
                        camera.ScreenToWorldPoint(new Vector2(screenDimentions.x, screenDimentions.y))
                    ),
                    mousePos
                );
            }
            camera.GetComponent<LineQueue>().NewLine(start, end, color);
        }
    }

    private void OnPreRender() {
        camera.GetComponent<LineQueue>().NewLine(start, end, color);
    }

    void Hide() {
        hidden = true;
    }
    void Show() {
        hidden = false;
    }
}
