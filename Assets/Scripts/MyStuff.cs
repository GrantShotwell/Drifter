using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyStuff {
    #region Geometry
    public class Geometry {
        #region Methods
        public static int left = -1, right = 1, up = 1, down = -1;
        public static int horizontal = 0, vertical = 1;

        public static int Direction(float input) {
            if(input >= 0) return 1;
            else return -1;
        }
        public static int AngleDirection(float angle) {
            angle = NormalizeDegree(angle);
            if(-90 > angle && angle > 90) return 1;
            else if(Mathf.Abs(angle) == 90) return 0;
            else return -1;
        }

        public static float ConvertToRadians(float degr) { return degr * (Mathf.PI / 180f); }
        public static float ConvertToDegrees(float radi) { return radi * (180f / Mathf.PI); }

        public static float GetAngle(Vector2 v1, Vector2 v2) { return GetAngle(v1.x, v1.y, v2.x, v2.y); }
        public static float GetAngle(float x1, float y1, float x2, float y2) { return ConvertToDegrees(Mathf.Atan2(y1 - y2, x1 - x2)); }
        
        //Todo: replace the 'while()' with math
        public static float NormalizeDegree(float degr) {
            while(degr < 0) degr += 360;
            while(degr > 360) degr -= 360;
            return degr;
        }
        public static float NormalizeDegree2(float degr) {
            while(degr < -360) degr += 360;
            while(degr > +360) degr -= 360;
            return degr;
        }

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

        public static float LawOfCosForAngleA(float a, float b, float c) { return LawOfCosForAngleC(c, b, a); }
        public static float LawOfCosForAngleB(float a, float b, float c) { return LawOfCosForAngleC(a, c, b); }
        public static float LawOfCosForAngleC(float a, float b, float c) { return Mathf.Acos((Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(c, 2)) / (2 * a * b)) * Mathf.Rad2Deg; }

        public static bool Exists(float number) { return (!float.IsInfinity(number)) && (!float.IsNaN(number)); }
        public static bool Exists(Vector2 vector) { return Exists(vector.x) && Exists(vector.y); }

        public static float LimitTo(float number, float limit) {
            if(Mathf.Abs(number) > Mathf.Abs(limit)) number = limit * Geometry.Direction(number);
            return number;
        }

        public static bool IsBetweenRange(float number, float range) {
            return -range <= number && number <= range;
        }

        public static Vector2 HeadToTailAngle(Vector2 v1, Vector2 v2, float degrees) {
            return v2.SetHeading(180.0f - v1.Heading() - degrees);
        }
        #endregion

        #region Classes
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
                return PointFromDistance(point, distance, Geometry.Direction(direction.x - point.x));
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

        public class Triangle {
            public float A, B, C; //angles
            public float a, b, c; //sides

            public Triangle(float _a, float _b, float _c) {
                a = _a;
                b = _b;
                c = _c;
                SolveForAngles();
            }

            public void SolveForAngles() {
                A = LawOfCosForAngleA(a, b, c);
                B = LawOfCosForAngleB(a, b, c);
                C = LawOfCosForAngleC(a, b, c);
            }

            public override string ToString() {
                return base.ToString();
            }
        }
        #endregion
    }
    #endregion

    #region Ranges
    [System.Serializable]
    public class Range {
        public static Range infinite = new Range(Mathf.NegativeInfinity, Mathf.Infinity);
        public float size => Mathf.Abs(max - min);

        public float min, max;
        public Range(float minimum = Mathf.NegativeInfinity, float maximum = Mathf.Infinity) {
            min = minimum;
            max = maximum;
        }

        public bool Contains(float value) {
            return min <= value && value <= max;
        }

        public float Place(float value) {
            if(min > value) value = min;
            if(max < value) value = max;
            return value;
        }

        public override string ToString() {
            return "[" + min + ", " + max + "]";
        }

        public static Range operator +(Range range, float value) { return new Range(range.min + value, range.max + value); }
        public static Range operator -(Range range, float value) { return new Range(range.min - value, range.max - value); }
        public static Range operator *(Range range, float value) { return new Range(range.min * value, range.max * value); }
        public static Range operator /(Range range, float value) { return new Range(range.min / value, range.max / value); }
        public static Range operator %(Range range, float value) { return new Range(range.min % value, range.max % value); }
        public static Range operator +(Range first, Range secnd) { return new Range(first.min + secnd.min, first.max + secnd.max); }
        public static Range operator -(Range first, Range secnd) { return new Range(first.min - secnd.min, first.max - secnd.max); }
        public static Range operator *(Range first, Range secnd) { return new Range(first.min * secnd.min, first.max * secnd.max); }
        public static Range operator /(Range first, Range secnd) { return new Range(first.min / secnd.min, first.max / secnd.max); }
        public static Range operator %(Range first, Range secnd) { return new Range(first.min % secnd.min, first.max % secnd.max); }
    }

    [System.Serializable]
    public class Range2D {
        public static Range2D infinite = new Range2D(Range.infinite, Range.infinite);
        public Range x, y;
        public Range2D(Range X, Range Y) {
            x = X;
            y = Y;
        }

        public bool Contains(Vector2 value) {
            return x.Contains(value.x) && y.Contains(value.y);
        }

        public Vector2 Place(Vector2 value) {
            return new Vector2(x.Place(value.x), y.Place(value.y));
        }

        public override string ToString() {
            return "[" + x + ", " + y + "]";
        }
    }

    [System.Serializable]
    public class AngleRange : Range {
        public const float lowest = -360, highest = 360;
        public static AngleRange unlimited = new AngleRange(lowest, highest);
        public AngleRange(float minimum = -360, float maximum = +360) : base(minimum, maximum) {
            while(min < -360) min += 360;
            while(max > +360) max -= 360;
        }

        public new bool Contains(float angle) {
            angle = Geometry.NormalizeDegree(angle);
            return base.Contains(angle) || base.Contains(angle - 360);
        }

        public new float Place(float angle) {
            angle = Geometry.NormalizeDegree(angle);
            if(base.Place(angle - 360) == angle) return angle - 360;
            return base.Place(angle);
        }

        public static AngleRange operator +(AngleRange range, float value) { return new AngleRange(range.min + value, range.max + value); }
        public static AngleRange operator -(AngleRange range, float value) { return new AngleRange(range.min - value, range.max - value); }
        public static AngleRange operator *(AngleRange range, float value) { return new AngleRange(range.min * value, range.max * value); }
        public static AngleRange operator /(AngleRange range, float value) { return new AngleRange(range.min / value, range.max / value); }
        public static AngleRange operator %(AngleRange range, float value) { return new AngleRange(range.min % value, range.max % value); }
        public static AngleRange operator +(AngleRange first, AngleRange secnd) { return new AngleRange(first.min + secnd.min, first.max + secnd.max); }
        public static AngleRange operator -(AngleRange first, AngleRange secnd) { return new AngleRange(first.min - secnd.min, first.max - secnd.max); }
        public static AngleRange operator *(AngleRange first, AngleRange secnd) { return new AngleRange(first.min * secnd.min, first.max * secnd.max); }
        public static AngleRange operator /(AngleRange first, AngleRange secnd) { return new AngleRange(first.min / secnd.min, first.max / secnd.max); }
        public static AngleRange operator %(AngleRange first, AngleRange secnd) { return new AngleRange(first.min % secnd.min, first.max % secnd.max); }

    }
    #endregion

    #region Camera Control
    [System.Serializable]
    public class CameraLimit {
        public Range2D position;
        public Range size;
        public bool lerpLimit;
        [ConditionalField("lerpLimit")]
        public float lerp;

        public CameraLimit(Range2D Position = null, Range Size = null) {
            position = Position;
            size = Size;
            lerpLimit = false;
        }

        public CameraLimit(Range2D Position, Range Size, float Lerp) {
            position = Position;
            size = Size;
            lerpLimit = true;
            lerp = Lerp;
        }
    }
    #endregion

    #region Actions
    public class TimedAction {
        public Action action;
        public float finalTime, startTime;

        public TimedAction(float delay, Action whenFinished) {
            action = whenFinished;
            if(Time.inFixedTimeStep) finalTime = Time.fixedTime + delay;
            else finalTime = Time.time + delay;
            startTime = finalTime;
        }

        public bool TimerDone() {
            if(Time.inFixedTimeStep) return Time.fixedTime >= finalTime;
            else return Time.time >= finalTime;
        }

        public bool Active() {
            if(Time.inFixedTimeStep) return startTime <= Time.fixedTime && Time.fixedTime <= finalTime;
            else return startTime <= Time.time && Time.time <= finalTime;
        }

        public void Invoke() {
            action();
        }
    }

    public class ContinuousAction : TimedAction {
        public ContinuousAction(float duration, float endDelay, Action duringTime) : base(endDelay, duringTime) {
            if(Time.inFixedTimeStep) startTime = Time.fixedTime + endDelay - duration;
            else startTime = Time.time + endDelay - duration;
        }
    }

    public class TimedActionTracker {
        List<TimedAction> timedActions = new List<TimedAction>();
        bool singleUse;

        public TimedActionTracker(bool deleteWhenDone = true) {
            singleUse = deleteWhenDone;
        }

        public void Update() {
            foreach(TimedAction timedAction in timedActions.ToArray()) {
                if(timedAction.Active()) timedAction.Invoke();
                if(timedAction.TimerDone()) {
                    timedAction.Invoke();
                    if(singleUse) timedActions.Remove(timedAction);
                }
            }
        }

        public void AddAction(TimedAction timedAction) {
            timedActions.Add(timedAction);
        }
    }
    #endregion
    
    public static class Debugger {
        public static void DrawPinwheel(Vector2 origin, float degree) { DrawPinwheel(origin, degree, 0, Color.white); }
        public static void DrawPinwheel(Vector2 origin, float degree, Color color) { DrawPinwheel(origin, degree, 0, color); }
        public static void DrawPinwheel(Vector2 origin, float degree, float displacement) { DrawPinwheel(origin, degree, displacement, Color.white); }
        public static void DrawPinwheel(Vector2 origin, float degree, float displacement, Color color) {
            Range range = new Range(0, 360) + displacement;
            float angle = range.min;
            Vector2 direction = Vector2.right.Rotate(angle);
            while(range.Contains(angle)) {
                Debug.DrawRay(origin, direction, color);
                direction = direction.Rotate(degree);
                angle += degree;
            }
        }
    }
}

public static class Vector2Extension {
    public static float Heading(this Vector2 v) {
        return Vector2.SignedAngle(Vector2.right, v);
    }

    public static Vector2 SetHeading(this Vector2 v, float degrees) {
        return new Vector2(v.magnitude, 0).Rotate(degrees);
    }

    public static Vector2 Rotate(this Vector2 v, float degrees) { //Stack Overflow
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        float x = v.x;
        float y = v.y;
        v.x = (cos * x) - (sin * y);
        v.y = (sin * x) + (cos * y);
        return v;
    }

    public static Vector2 RotateTo(this Vector2 v1, Vector2 v2, float degrees) {
        if(Vector2.SignedAngle(v1, v2) > 0) return Rotate(v1, degrees);
        else return Rotate(v1, -degrees);
    }

    public static Vector2 RotateFrom(this Vector2 v1, Vector2 v2, float degrees) {
        if(Vector2.SignedAngle(v1, v2) < 0) return Rotate(v1, degrees);
        else return Rotate(v1, -degrees);
    }

    public static Vector2 FromRotation(this Vector2 v1, Vector2 v2, float degrees) {
        float rotation = Vector2.SignedAngle(Vector2.right, v2);
        return v1.SetHeading(degrees + rotation);
    }

    public static Vector2 Set(this Vector2 v, Vector2 newVector) {
        v.Set(newVector.x, newVector.y);
        return v;
    }
}
