using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geometry {
    public static int left = -1, right = 1, up = 1, down = -1;
    public static int horizontal = 0, vertical = 1;

    public static int direction(float input) {
        if(input > 0) return 1;
        else return -1;
    }

    public static float ConvertToRadians(float degr) { return degr * (Mathf.PI / 180f); }
    public static float ConvertToDegrees(float radi) { return radi * (180f / Mathf.PI); }

    public static float GetAngle(Vector2 v1, Vector2 v2) { return GetAngle(v1.x, v1.y, v2.x, v2.y); }
    public static float GetAngle(float x1, float y1, float x2, float y2) { return ConvertToDegrees(Mathf.Atan2(y1 - y2, x1 - x2)); }

    public static float RoundToMultiple(float number, float multiple) { return Mathf.RoundToInt(number / multiple) * multiple; }

    public static Line LineFromTwoPoints(Vector2 p1, Vector2 p2) {
        float a = (p1.y - p2.y) / (p1.x - p2.x); //m
        float c = p1.y - (a * p1.x); //b
        if(float.IsInfinity(a)) return new Line(-a, 1, c, p1.x);
        else return new Line(-a, 1, c);
    }
    public static Line LineFromAngle(Vector2 p1, float degr) {
        float a = Mathf.Tan(ConvertToRadians(degr));
        float c = p1.y - (a * p1.x);
        if(float.IsInfinity(a)) return new Line(-a, 1, c, p1.x);
        else return new Line(-a, 1, c);
    }
    public static Line LineFromShift(Vector2 d, Line line) {
        Vector2 p1 = new Vector2(1, line.YFromX(1));
        Vector2 p2 = new Vector2(2, line.YFromX(2));
        p1 += d; p2 += d;
        return LineFromTwoPoints(p1, p2);
    }

    public static bool AreParallel(Line l1, Line l2) {
        float delta = (l1.a * l2.b) - (l2.a * l1.b);
        if(delta == 0) return true;
        else return false;
    }

    public static Vector2 Intersection(Line l1, Line l2) {
        if(l1.isVertical != l2.isVertical) {
            if(l1.isVertical) {
                return new Vector2(l1.X, l2.YFromX(l1.X));
            }
            if(l2.isVertical) {
                return new Vector2(l2.X, l1.YFromX(l2.X));
            }
        }
        float delta = (l1.a * l2.b) - (l2.a * l1.b);
        float x = (l2.b * l1.c - l1.b * l2.c) / delta;
        float y = (l1.a * l2.c - l2.a * l1.c) / delta;
        return new Vector2(x, y);
    }
}

public class Line {
    public float a, b, c; //Ax + By = C
    public float X = 0;
    public bool isVertical;
    public Line(float A, float B, float C, float x) {
        isVertical = true;
        a = A;
        b = B;
        c = C;
        X = x;
    }
    public Line(float A, float B, float C) {
        isVertical = false;
        a = A;
        b = B;
        c = C;
    }
    public float Slope() { return -a; }
    public float Angle() { return Geometry.ConvertToDegrees(Mathf.Atan(Slope())); }
    public float YFromX(float x) {
        return (c - a * x) / b;
    }
    public float XFromY(float y) {
        if(isVertical) return X;
        else return (c - b * y) / a;
    }
    public void ShiftX(float d) {
        if(isVertical) X += d;
        else c += d * a;
    }
    public void ShiftY(float d) {
        if(!isVertical) c += d * b;
    }

    public Vector2 PointFromX(float x) {
        return new Vector2(x, YFromX(x));
    }
    public Vector2 PointFromY(float y) {
        return new Vector2(XFromY(y), y);
    }

    public Vector2 PointFromDistance(Vector2 point, float distance, Vector2 direction) {
        return PointFromDistance(point, distance, Geometry.direction(direction.x - point.x));
    }
    public Vector2 PointFromDistance(Vector2 point, float distance, int direction) {
        float theta = Geometry.ConvertToRadians(Angle()) * direction;
        Vector2 point2 = new Vector2(Mathf.Cos(theta) * distance, Mathf.Sin(theta) * distance);
        point2 = new Vector2(point2.x * direction, point2.y);
        point2 += point;
        return point2;
    }
    
    public Vector2 Intersection(Line l2) {
        Line l1 = this;
        if(l1.isVertical != l2.isVertical) {
            if(l1.isVertical) {
                return new Vector2(l1.X, l2.YFromX(l1.X));
            }
            if(l2.isVertical) {
                return new Vector2(l2.X, l1.YFromX(l2.X));
            }
        }
        float delta = (l1.a * l2.b) - (l2.a * l1.b);
        float x = (l2.b * l1.c - l1.b * l2.c) / delta;
        float y = (l1.a * l2.c - l2.a * l1.c) / delta;
        return new Vector2(x, y);
    }
}
