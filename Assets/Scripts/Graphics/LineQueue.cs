using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineQueue : MonoBehaviour {
    /// <summary>
    /// Purpose: ability to draw pixel lines from almost anywhere (FixedUpdate, Update, ect.)
    /// While this works from anywhere, if you need the line to have no delay (ex. line from player to mouse) then call this on OnPreRender()
    /// Uses the public class LinePainer to draw lines.
    /// </summary>

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

    private void OnPostRender() {
        for(int j = 0; j < lines; j++)
            LinePainter.Line(startPoints[j], endPoints[j], defaultMaterial, colors[j]);
        startPoints.Clear();
        endPoints.Clear();
        colors.Clear();
        lines = 0;
    }
}

public class LinePainter {
    Material defaultMaterial;
    Color defaultColor;

    LinePainter(Material m, Color c) {
        defaultMaterial = m;
        defaultColor = c;
    }

    public void Line(Vector2 start, Vector2 end) {
        Line(start, end, defaultMaterial, defaultColor);
    }

    public static void Line(Vector2 start, Vector2 end, Material material, Color color) {
        Line(start, end, material, color, Camera.main);
    }
    public static void Line(Vector2 start, Vector2 end, Material material, Color color, Camera camera) {
        Vector2 screenDimentions = new Vector2(Screen.width, Screen.height);
        GL.PushMatrix();
        material.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex(camera.WorldToScreenPoint(start) / screenDimentions);
        GL.Vertex(camera.WorldToScreenPoint(end) / screenDimentions);
        GL.End();
        GL.PopMatrix();
    }
}
