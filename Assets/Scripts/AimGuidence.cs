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
    Vector2 hitpoint;
    RaycastHit2D hit;

    void Start () {
        player = GameObject.Find("Player");
        camera = GetComponent<Camera>();
	}

    void LateUpdate() {
        mousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        playerPos = player.transform.position;
        camPos = transform.position;
        hit = Physics2D.Raycast(playerPos, mousePos - camPos, Mathf.Infinity, ~(1 << 8));
        hitpoint = hit.point;
    }
	
	void OnPostRender() {
        if(!hidden) {
            Line rayLine = Geometry.LineFromTwoPoints(playerPos, mousePos);
            if(!rayLine.isVertical) {
                GL.PushMatrix();
                material.SetPass(0);
                GL.LoadOrtho();
                GL.Begin(GL.LINES);
                GL.Color(color);
                GL.Vertex(camera.WorldToScreenPoint(playerPos) / screenDimentions);
                if(hit.collider != null) GL.Vertex(camera.WorldToScreenPoint(hitpoint) / screenDimentions);
                else GL.Vertex(camera.WorldToScreenPoint(rayLine.PointFromDistance(playerPos, Vector2.Distance(camera.ScreenToWorldPoint(new Vector2(0, 0)), camera.ScreenToWorldPoint(new Vector2(screenDimentions.x, screenDimentions.y))), mousePos)) / screenDimentions);
                GL.End();
                GL.PopMatrix();
            }
        }
    }

    void Hide() {
        hidden = true;
    }
    void Show() {
        hidden = false;
    }
}
