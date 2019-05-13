using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;
using MyStuff.GeometryObjects;

/// <summary>
/// Purpose: ability to draw pixel lines from almost anywhere (FixedUpdate, Update, ect.).
/// While this works from anywhere, if you need the line to have no delay (ex. a line from player to mouse) then call this on OnPreRender().
/// Uses the public class LinePainer to draw lines.
/// </summary>
public class LineQueue : MonoBehaviour {

    public Material defaultMaterial;
    public Color defaultColor;
    int lines = 0;
    List<Vector2> startPoints = new List<Vector2>();
    List<Vector2> endPoints = new List<Vector2>();
    List<Color> colors = new List<Color>();

    public void NewLine(Vector2 start, Vector2 end) { NewLine(start, end, defaultMaterial, defaultColor); }
    public void NewLine(Vector2 start, Vector2 end, Color color) { NewLine(start, end, defaultMaterial, color); }
    public void NewLine(Vector2 start, Vector2 end, Material material) { NewLine(start, end, material, defaultColor); }
    public void NewLine(Vector2 start, Vector2 end, Material material, Color color) {
        startPoints.Add(start);
        endPoints.Add(end);
        colors.Add(color);
        lines++;
    }

    public void NewLineFromPlayer(Vector2 position, Vector2 end, Color color) {
        float angle = Geometry.GetAngle(position, end) + Mathf.PI;
        Vector2 start = Geometry.Vector2FromAngle(angle, 0.75f);
        start = Range2D.half.Place(start) + position;
        NewLine(start, end, color);
    }

    private void OnPostRender() {
        for(int j = 0; j < lines; j++)
            LinePainter.Line(startPoints[j], endPoints[j], defaultMaterial, colors[j]);
        startPoints.Clear();
        endPoints.Clear();
        colors.Clear();
        lines = 0;
    }
}

public static class LinePainter {

    public static void Line(Vector2 start, Vector2 end, Material material, Color color) {
        Line(start, end, material, color, Camera.main);
    }

    public static void Line(Vector2 start, Vector2 end, Material material, Color color, Camera camera) {
        Vector2 screenDimentions = new Vector2(Screen.width, Screen.height);
        
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        material.SetPass(0);
        GL.Color(color);
        GL.Vertex(camera.WorldToScreenPoint(start) / screenDimentions);
        GL.Vertex(camera.WorldToScreenPoint(end) / screenDimentions);
        GL.End();
    }
}
