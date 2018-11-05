using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyStuff {
    #region Geometry
    /// <summary>
    /// A collection of math functions, shape classes, and enumerators related to Geometry.
    /// </summary>
    public class Geometry {
        #region Methods
        /// <summary>General direction in terms of left/right/up/down.</summary>
        public enum Direction { Up = 1, Down = -1, Right = 1, Left = -1, }

        /// <summary>Positive = 1, Negative = -1.</summary>
        public enum Sign { Positive = 1, Negative = -1 }

        /// <summary>x : y : : horizontal : vertical</summary>
        public enum Axis { Horizontal = 0, Vertical = 1 }

        /// <summary>
        /// On a 1-Dimentional line, the direction from X1 to X2 is equal to the sign of the input.
        /// input = X1 - X2
        /// </summary>
        /// <param name="input">X1 - X2</param>
        /// <returns>Returns the sign of the input (-1 or +1).</returns>
        public static int LinearDirection(float input) {
            if(input >= 0) return 1;
            else return -1;
        }
        
        /// <summary>
        /// Gives the Geometry.Direction of an angle relative to the given Geometry.Axis.
        /// </summary>
        /// <param name="angle">The input angle in degrees.</param>
        /// <param name="axis">The axis to make the output direction relative to.</param>
        /// <returns>Axis.Horizontal relates to Quadrant 1 and 2 for positive. Axis.Vertical relates to Quadrant 1 and 4 for positive.</returns>
        public static Direction AngleDirection(float angle, Axis axis = Axis.Horizontal) {
            angle = NormalizeDegree3(angle);
            if(axis == Axis.Horizontal) {
                if(-90 > angle && angle > 90) return Direction.Right;
                if(angle == 0 || angle == 180) return 0;
                else return Direction.Left;
            }
            if(axis == Axis.Vertical) {
                if(0 > angle && angle > 180) return Direction.Up;
                if(angle == 0 || angle == 180) return 0;
                else return Direction.Down;
            }
            return 0;
        }

        public static float ConvertToRadians(float degr) { return degr * (Mathf.PI / 180f); }
        public static float ConvertToDegrees(float radi) { return radi * (180f / Mathf.PI); }

        /// <summary>
        /// Returns the angle between v1 and v2 with respect to the X/Y axies.
        /// </summary>
        /// <param name="v1">Vector 1 (angle from)</param>
        /// <param name="v2">Vector 2 (angle to)</param>
        public static float GetAngle(Vector2 v1, Vector2 v2) { return GetAngle(v1.x, v1.y, v2.x, v2.y); }
        public static float GetAngle(float x1, float y1, float x2, float y2) { return ConvertToDegrees(Mathf.Atan2(y1 - y2, x1 - x2)); }

        //todo: replace the 'while()' with math
        /// <summary>
        /// Normalizes an angle on the interval [0, 360].
        /// </summary>
        /// <param name="degr">Input angle in degrees.</param>
        /// <returns>Returns an equivalent angle on the interval [0, 360].</returns>
        public static float NormalizeDegree1(float degree) {
            while(degree <   0) degree += 360;
            while(degree > 360) degree -= 360;
            return degree;
        }

        /// <summary>
        /// Normalizes an angle on the interval [-360, +360].
        /// </summary>
        /// <param name="degr">Input angle in degrees.</param>
        /// <returns>Returns an equivalent angle on the interval [-360, +360].</returns>
        public static float NormalizeDegree2(float degree) {
            while(degree < -360) degree += 360;
            while(degree > +360) degree -= 360;
            return degree;
        }

        /// <summary>
        /// Normalizes an angle on the interval [-180, +180]
        /// </summary>
        /// <param name="degree">Input angle in degrees.</param>
        /// <returns>Returns an equivalent angle on the interval [-180, +180].</returns>
        public static float NormalizeDegree3(float degree) {
            while(degree < -180) degree += 360;
            while(degree > +180) degree -= 360;
            return degree;
        }

        /// <summary>
        /// Rounds the given number to the nearest multiple of another number.
        /// </summary>
        /// <param name="number">Input number.</param>
        /// <param name="multiple">Input number will be rounded to some (multiple * n).</param>
        /// <returns>Returns the multiple closest to number.</returns>
        public static float RoundToMultiple(float number, float multiple) { return Mathf.RoundToInt(number / multiple) * multiple; }

        /// <summary>
        /// Generates a line given two points.
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>Returns a line that intersects the two given points.</returns>
        public static Line LineFromTwoPoints(Vector2 point1, Vector2 point2) {
            float a = (point1.y - point2.y) / (point1.x - point2.x);
            float c = point1.y - (a * point1.x);
            if(float.IsInfinity(a)) return new Line(-a, 1, c, point1.x);
            else return new Line(-a, 1, c);
        }

        /// <summary>
        /// Generates a line given one point and an angle.
        /// </summary>
        /// <returns></returns>
        public static Line LineFromAngle(Vector2 point, float degr) {
            float a = Mathf.Tan(ConvertToRadians(degr));
            float c = point.y - (a * point.x);
            if(float.IsInfinity(a)) return new Line(-a, 1, c, point.x);
            else return new Line(-a, 1, c);
        }

        /// <summary>
        /// Generates a line identical to the original, but shifted left/right by distance.
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="line"></param>
        public static Line LineFromShift(Vector2 distance, Line line) {
            Vector2 p1 = new Vector2(1, line.YFromX(1));
            Vector2 p2 = new Vector2(2, line.YFromX(2));
            p1 += distance; p2 += distance;
            return LineFromTwoPoints(p1, p2);
        }

        /// <summary>
        /// Are these two lines parallel?
        /// </summary>
        /// <returns>Returns 'true' if (l1.a * l2.b) - (l2.a * l1.b) == 0.</returns>
        public static bool AreParallel(Line line1, Line line2) {
            float delta = (line1.a * line2.b) - (line2.a * line1.b);
            if(delta == 0) return true;
            else return false;
        }

        /// <summary>
        /// Finds the intersection between two Geometry objects.
        /// </summary>
        /// <param name="l1">Line 1</param>
        /// <param name="l2">Line 2</param>
        /// <returns>Returns the intersection between l1 and l2.</returns>
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
        
        /// <summary>
        /// Uses the Law of Cosines to find an angle of a triangle given only the side lenghs.
        /// </summary>
        /// <param name="a">Side length A.</param>
        /// <param name="b">Side length B.</param>
        /// <param name="c">Side length C.</param>
        /// <returns>Returns the angle opposite of side a.</returns>
        public static float LawOfCosForAngleA(float a, float b, float c) { return LawOfCosForAngleC(c, b, a); }

        /// <summary>
        /// Uses the Law of Cosines to find an angle of a triangle given only the side lenghs.
        /// </summary>
        /// <param name="a">Side length A.</param>
        /// <param name="b">Side length B.</param>
        /// <param name="c">Side length C.</param>
        /// <returns>Returns the angle opposite of side b.</returns>
        public static float LawOfCosForAngleB(float a, float b, float c) { return LawOfCosForAngleC(a, c, b); }

        /// <summary>
        /// Uses the Law of Cosines to find an angle of a triangle given only the side lenghs.
        /// </summary>
        /// <param name="a">Side length A.</param>
        /// <param name="b">Side length B.</param>
        /// <param name="c">Side length C.</param>
        /// <returns>Returns the angle opposite of side c.</returns>
        public static float LawOfCosForAngleC(float a, float b, float c) { return Mathf.Acos((Mathf.Pow(a, 2) + Mathf.Pow(b, 2) - Mathf.Pow(c, 2)) / (2 * a * b)) * Mathf.Rad2Deg; }

        /// <summary>
        /// Determines if the given value exists.
        /// </summary>
        /// <returns>Returns 'true' if the number is not Infinity and is not NaN.</returns>
        public static bool Exists(float number) { return (!float.IsInfinity(number)) && (!float.IsNaN(number)); }

        /// <summary>
        /// Determines if the given value exists.
        /// </summary>
        /// <returns>Returns 'true' if the components are not Infinity nor NaN.</returns>
        public static bool Exists(Vector2 vector) { return Exists(vector.x) && Exists(vector.y); }
        
        public static float LimitTo(float input, float limit) {
            if(Mathf.Abs(input) > Mathf.Abs(limit)) input = limit * Geometry.LinearDirection(input);
            return input;
        }

        /// <summary>
        /// Checks if the input is on the interval [-range, +range].
        /// </summary>
        /// <param name="range">[-range, +range]</param>
        /// <returns>Returns true if input is on the interval [-range, +range]</returns>
        public static bool IsBetweenRange(float input, float range) {
            return -range <= input && input <= range;
        }

        /// <summary>
        /// Changes the heading of v2 so that if v2's tail is placed on the head of v1, the angle between those two vectors is 'degrees'.
        /// This can also be described as setting the angle of a bend in a line.
        /// </summary>
        /// <param name="v1">Unchanged vector.</param>
        /// <param name="v2">Changed vector.</param>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>Returns v2.SetHeading(180.0f - v1.Heading() - degrees).</returns>
        public static Vector2 HeadToTailAngle(Vector2 v1, Vector2 v2, float degrees) {
            return v2.SetHeading(180.0f - v1.Heading() - degrees);
        }
        #endregion

        #region Classes
        /// <summary>
        /// A line.
        /// </summary>
        public class Line {
            /// <summary>ax + by = c;</summary>
            public float a, b, c; //Ax + By = C

            /// <summary>If the line is horizontal, the equation of the line is X = [a constant].</summary>
            public float X = 0;

            /// <summary>Is the line horizontal?</summary>
            public bool isVertical;

            /// <summary>
            /// Creates a vertical line given A, B, C, and it's x-coordinate.
            /// </summary>
            /// <param name="A">Ax + by = c</param>
            /// <param name="B">ax + By = c</param>
            /// <param name="C">ax + by = C</param>
            /// <param name="x">X = this (horizontal line)</param>
            public Line(float A, float B, float C, float x) {
                isVertical = true;
                a = A;
                b = B;
                c = C;
                X = x;
            }

            /// <summary>
            /// Creates a line given A, B, and C in Ax + By = C
            /// </summary>
            /// <param name="A">Ax + by = c</param>
            /// <param name="B">ax + By = c</param>
            /// <param name="C">ax + by = C</param>
            public Line(float A, float B, float C) {
                isVertical = false;
                a = A;
                b = B;
                c = C;
            }
            
            /// <summary>the slope of the line</summary>
            public float Slope() => -a;

            /// <summary>angle of the slope in degrees</summary>
            public float Angle() { return ConvertToDegrees(Mathf.Atan(Slope())); }
            
            public float YFromX(float x) {
                return (c - a * x) / b;
            }
            public float XFromY(float y) {
                if(isVertical) return X;
                else return (c - b * y) / a;
            }

            /// <summary>
            /// Shifts the line left/right by a distance of 'd'.
            /// </summary>
            /// <param name="d">distance to shift the line</param>
            public void ShiftX(float d) {
                if(isVertical) X += d;
                else c += d * a;
            }

            /// <summary>
            /// Shifts the line upwards/downwards by a distance of 'd'.
            /// </summary>
            /// <param name="d">distance to shift the line.</param>
            public void ShiftY(float d) {
                if(!isVertical) c += d * b;
            }

            /// <summary>Returns (x, y) given x.</summary>
            /// <param name="x">the x-coordinate of the point</param>
            public Vector2 PointFromX(float x) {
                return new Vector2(x, YFromX(x));
            }

            /// <summary>Returns (x, y) given y.</summary>
            /// <param name="y">the y-coordinate of the point</param>
            public Vector2 PointFromY(float y) {
                return new Vector2(XFromY(y), y);
            }

            /// <summary>
            /// Finds a point on this line from a start point, distance, and direction.
            /// </summary>
            /// <param name="point">distance from this point</param>
            /// <param name="distance">distance from point 'point'</param>
            /// <param name="direction">direction on the line (does not need to align with the lign)</param>
            /// <returns></returns>
            public Vector2 PointFromDistance(Vector2 point, float distance, Vector2 direction) {
                return PointFromDistance(point, distance, Geometry.LinearDirection(direction.x - point.x));
            }

            /// <summary>
            /// Finds a point on this line from a start point, distance, and direction.
            /// </summary>
            /// <param name="point">Distance from this point.</param>
            /// <param name="distance">Distance from point 'point'.</param>
            /// <param name="direction">Left = -1, Right = +1.</param>
            /// <returns></returns>
            public Vector2 PointFromDistance(Vector2 point, float distance, int direction) {
                float theta = Geometry.ConvertToRadians(Angle()) * direction;
                Vector2 point2 = new Vector2(Mathf.Cos(theta) * distance, Mathf.Sin(theta) * distance);
                point2 = new Vector2(point2.x * direction, point2.y);
                point2 += point;
                return point2;
            }

            /// <summary>
            /// Finds the intersection of this and l2.
            /// </summary>
            /// <returns>Returns the point of intersection.</returns>
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

        /// <summary>
        /// A simple triangle.
        /// </summary>
        public class Triangle {
            public float A, B, C; //angles
            public float a, b, c; //sides

            /// <summary>
            /// Returns a complete triangle given the side lengths.
            /// </summary>
            /// <param name="_a">side length 'a'</param>
            /// <param name="_b">side length 'b'</param>
            /// <param name="_c">side length 'c'</param>
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
    /// <summary>
    /// Class for an inclusive range.
    /// </summary>
    [System.Serializable]
    public class Range {
        public static Range infinite = new Range(Mathf.NegativeInfinity, Mathf.Infinity);
        public float size => Mathf.Abs(max - min);
        public float random => UnityEngine.Random.Range(min, max);

        public float min, max;
        public Range(float minimum = Mathf.NegativeInfinity, float maximum = Mathf.Infinity) {
            min = minimum;
            max = maximum;
        }

        /// <summary>
        /// Checks if the given value is within the range.
        /// </summary>
        /// <returns>Returns 'true' if the given value is on the interval [min, max].</returns>
        public bool Contains(float value) {
            return min <= value && value <= max;
        }

        /// <summary>
        /// Places the given value to the nearest value on the interval [min, max].
        /// </summary>
        /// <returns>If the given value is lesser/greater than min/max then it returns min/max. Otherwise, the value is unchanged.</returns>
        public float Place(float value) {
            if(min > value) value = min;
            if(max < value) value = max;
            return value;
        }

        public override string ToString() {
            return "[" + min + ", " + max + "]";
        }

        #region Operators
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
        #endregion
    }

    /// <summary>
    /// Class for an inclusive Vector2 range for x and y components.
    /// </summary>
    [System.Serializable]
    public class Range2D {
        /// <summary>Unlimited range.</summary>
        public static Range2D infinite = new Range2D(Range.infinite, Range.infinite);

        /// <summary>The range of this vector component.</summary>
        public Range x, y;

        /// <summary>
        /// Creates a new Range2D.
        /// </summary>
        /// <param name="x">Range for the x component.</param>
        /// <param name="y">Range for the y component.</param>
        public Range2D(Range x, Range y) {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Checks if the given value is within the range.
        /// </summary>
        /// <returns>Returns 'true' if the given value is on the interval [min, max].</returns>
        public bool Contains(Vector2 value) {
            return x.Contains(value.x) && y.Contains(value.y);
        }

        /// <summary>
        /// Applies 'Range.Place(float)' to both components of the given vector to their respective range.
        /// </summary>
        /// <returns>Returns a new Vector2(x.Place(value.x), y.Place(value.y))</returns>
        public Vector2 Place(Vector2 value) {
            return new Vector2(x.Place(value.x), y.Place(value.y));
        }

        public override string ToString() {
            return "[" + x + ", " + y + "]";
        }
    }

    /// <summary>
    /// Extention of Range that measures angle total displacement.
    /// </summary>
    [System.Serializable]
    public class AngleRange : Range {
        /// <summary>The allowed range for any AngleRange.</summary>
        public const float Lowest = -360, Highest = 360;
        /// <summary>The allowed range for any AngleRange.</summary>
        public static AngleRange Full = new AngleRange(Lowest, Highest);

        /// <summary>
        /// Creates a new AngleRange with the specified minimum and maximum.
        /// </summary>
        /// <param name="minimum">The minimum value of the range. Cannot be greater than maximum, and cannot be lower than -360.</param>
        /// <param name="maximum">The maximum value of the range. Cannot be less than minimum, and cannot be higher than +360.</param>
        public AngleRange(float minimum = -360, float maximum = +360) : base(minimum, maximum) {
            while(min < -360) min += 360;
            while(max > +360) max -= 360;
        }

        /// <summary>
        /// The angle is normalized to [0, 360] and then checks if it lies within the range.
        /// If not, then it also checks if (angle - 360) lies within the range.
        /// </summary>
        public new bool Contains(float angle) {
            angle = Geometry.NormalizeDegree1(angle);
            return base.Contains(angle) || base.Contains(angle - 360);
        }

        /// <summary>
        /// Places the given value to the nearest value on the interval [min, max].
        /// </summary>
        /// <returns>If the given value is lesser/greater than min/max then it returns min/max. Otherwise, the value is unchanged.</returns>
        public new float Place(float angle) {
            angle = Geometry.NormalizeDegree1(angle);
            if(base.Place(angle - 360) == angle) return angle - 360;
            return base.Place(angle);
        }

        /// <summary>
        /// Normalizes an angle to [0, 360].
        /// </summary>
        /// <param name="angle">Angle to normalize.</param>
        /// <returns>Returns an equivalent angle on the interval [0, 360]</returns>
        public static float Normalize(float angle) {
            return angle - ((int)(angle / 360) * 360f);
        }

        #region Operators
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
        #endregion
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
    /// <summary>
    /// Class for actions on a delay.
    /// </summary>
    public class TimedAction {
        public Action action;
        public float startTime, finalTime;
        public string tag;

        /// <summary>
        /// Creates a action that will be run after the delay is over.
        /// </summary>
        /// <param name="delay">Time (in seconds) the action is delayed.</param>
        /// <param name="whenFinished">Action to run when the delay is over.</param>
        public TimedAction(float delay, Action whenFinished) : this(delay, "default", whenFinished) {}

        /// <summary>
        /// Creates an action that will be run after the delay is over.
        /// </summary>
        /// <param name="delay">Time (in seconds) that the action is delayed.</param>
        /// <param name="tag">Optional identifier tag. (default = "default")</param>
        /// <param name="whenFinished">Action to run when the delay is over.</param>
        public TimedAction(float delay, string tag, Action whenFinished) {
            action = whenFinished;
            if(Time.inFixedTimeStep) finalTime = Time.fixedTime + delay;
            else finalTime = Time.time + delay;
            startTime = finalTime;
            this.tag = tag;
        }

        /// <summary>
        /// Checks if the timer is complete.
        /// </summary>
        /// <returns>Returns true if Time.[fixedTime/time] is greater than finalTime.</returns>
        public bool TimerDone() {
            if(Time.inFixedTimeStep) return Time.fixedTime >= finalTime;
            else return Time.time >= finalTime;
        }

        /// <summary>
        /// Checks if the timer is active.
        /// </summary>
        /// <returns>Returns 'true' if Time.[fixedTime/Time] is within the time this action is allowed to be active.</returns>
        public bool Active() {
            if(Time.inFixedTimeStep) return startTime <= Time.fixedTime && Time.fixedTime <= finalTime;
            else return startTime <= Time.time && Time.time <= finalTime;
        }

        /// <summary>
        /// Immediately invokes this timer's action.
        /// </summary>
        public void Invoke() {
            action();
        }
    }

    /// <summary>
    /// Extention of TimedAction that allows for invoking multiple times over a set period of time.
    /// </summary>
    public class ContinuousAction : TimedAction {
        public ContinuousAction(float duration, Action duringTime) : this(0.0f, duration, duringTime) {}
        public ContinuousAction(float duration, string tag, Action duringTime) : this(0.0f, duration, tag, duringTime) {}
        public ContinuousAction(float delay, float duration, Action duringTime) : this(delay + duration, duration, "default", duringTime) {}
        public ContinuousAction(float delay, float duration, string tag, Action duringTime) : base(delay + duration, tag, duringTime) {
            if(Time.inFixedTimeStep) startTime = Time.fixedTime + delay;
            else startTime = Time.time + delay;
        }
    }

    /// <summary>
    /// Class for making use of TimedActions.
    /// </summary>
    public class TimedActionTracker {
        List<TimedAction> timedActions = new List<TimedAction>();
        bool singleUse;
        
        /// <summary>
        /// Keeps track of, and invokes, TimedActions by calling the Update() function from somewhere else.
        /// </summary>
        /// <param name="deleteWhenDone">Delete the TimedAction from the list when finished?</param>
        public TimedActionTracker(bool deleteWhenDone = true) {
            singleUse = deleteWhenDone;
        }

        /// <summary>
        /// Function to be called by another script in Monobehavior.Update() or Monobehavior.FixedUpdate().
        /// This object will not do anything untill this is called.
        /// </summary>
        public void Update() {
            foreach(TimedAction timedAction in timedActions.ToArray()) {
                if(timedAction.Active()) timedAction.Invoke();
                if(timedAction.TimerDone()) {
                    timedAction.Invoke();
                    if(singleUse) timedActions.Remove(timedAction);
                }
            }
        }

        /// <summary>
        /// Add a TimedAction to the list.
        /// </summary>
        /// <param name="timedAction">The TimedAction to add to the list.</param>
        public void AddAction(TimedAction timedAction) { timedActions.Add(timedAction); }

        /// <summary>
        /// Remove TimedActions from the list by tag.
        /// </summary>
        /// <param name="tag">All TimedActions with this tag will be removed from the list.</param>
        public void RemoveTagged(string tag) { foreach(TimedAction action in timedActions) if(action.tag == tag) timedActions.Remove(action); }
    }
    #endregion

    #region Debug
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
    #endregion

    #region MyFunctions
    public static class MyFunctions {
        /// <summary>
        /// Chooses a random item in a T[] array.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <returns>Returns a random item in the given array using Random.Range(0, array.Length-1). If the array is null, default(T) is returned.</returns>
        public static T RandomItem<T>(T[] array) {
            if(array == null) return default(T);
            return array[UnityEngine.Random.Range(0, array.Length - 1)];
        }
        
        /// <summary>
        /// Finds the children of a GameObject.
        /// </summary>
        /// <param name="gameObject">The parent GameObject.</param>
        /// <returns>Returns a GameObject[] array of all the children of the parent.</returns>
        public static GameObject[] GetChildren(GameObject gameObject) { return GetChildren(gameObject.transform); }

        /// <summary>
        /// Finds the children of a Transform.
        /// </summary>
        /// <param name="transform">The parent Transform.</param>
        /// <returns>Returns a GameObject[] array of all the children of the parent.</returns>
        public static GameObject[] GetChildren(Transform transform) {
            GameObject[] children = new GameObject[transform.childCount];
            for(int j = 0; j < children.Length; j++)
                children[j] = transform.GetChild(j).gameObject;
            return children;
        }

        /// <summary>
        /// Checks if called within fixed or update time, and returns the respective time.
        /// </summary>
        public static float time {
            get {
                if(Time.inFixedTimeStep) return Time.fixedTime;
                else return Time.time;
            }
        }
    }
    #endregion

    /// <summary>
    /// Update, FixedUpdate, or None
    /// </summary>
    public enum UpdateLocation { Update, FixedUpdate, None }
}

public static class Vector2Extension {
    /// <summary>
    /// The heading (in degrees) of the Vector. (same as Vector2.SignedAngle(Vector2.right, Vector2 v)
    /// </summary>
    /// <returns>Returns the degrees of this vector from Vector2.right.</returns>
    public static float Heading(this Vector2 v) { return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg; }

    /// <summary>
    /// Sets the heading of the vector.
    /// </summary>
    /// <param name="v">Original vector.</param>
    /// <param name="degrees">Amount of rotation in degrees.</param>
    /// <returns>Returns a new vector is a heading of v.Heading + degrees and magnitude of the original.</returns>
    public static Vector2 SetHeading(this Vector2 v, float degrees)     { return new Vector2(v.magnitude, 0).Rotate(degrees); }

    /// <summary>
    /// Sets the magnitude of the vector.
    /// </summary>
    /// <param name="v">Original vector.</param>
    /// <param name="magnitude">New magnitude of the vector.</param>
    /// <returns>Returns a new vector with the heading of the original but with the new magnitude.</returns>
    public static Vector2 SetMagnitude(this Vector2 v, float magnitude) { return new Vector2(magnitude, 0).Rotate(v.Heading()); }

    /// <summary>
    /// Rotates a vector by an angle.
    /// </summary>
    /// <param name="vector">Vector to rotate.</param>
    /// <param name="degrees">Rotation in degrees.</param>
    /// <returns>Returns a vector of the same magnitude, but rotated.</returns>
    public static Vector2 Rotate(this Vector2 vector, float degrees) { //Stack Overflow
        float rad = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);
        vector.x = (cos * vector.x) - (sin * vector.y);
        vector.y = (sin * vector.x) + (cos * vector.y);
        return vector;
    }

    /// <summary>
    /// Rotates v1 towards v2.
    /// </summary>
    /// <param name="v1">Original vector.</param>
    /// <param name="v2">Vector to.</param>
    /// <param name="degrees">Amount of rotation in degrees.</param>
    /// <returns>Returns v1.Rotate([+/-]degrees).</returns>
    public static Vector2 RotateTo(this Vector2 v1, Vector2 v2, float degrees) {
        if(Vector2.SignedAngle(v1, v2) > 0) return Rotate(v1, degrees);
        else return Rotate(v1, -degrees);
    }

    /// <summary>
    /// Rotates v1 away from v2.
    /// </summary>
    /// <param name="v1">Original vector.</param>
    /// <param name="v2">Vector from.</param>
    /// <param name="degrees">Amount of rotation in degrees.</param>
    /// <returns>Returns v1.Rotate([-/+]degrees).</returns>
    public static Vector2 RotateFrom(this Vector2 v1, Vector2 v2, float degrees) {
        if(Vector2.SignedAngle(v1, v2) < 0) return Rotate(v1, degrees);
        else return Rotate(v1, -degrees);
    }

    /// <summary>
    /// Sets the heading of v1 equal to the heading of v2, then rotates it.
    /// </summary>
    /// <param name="v1">Original vector.</param>
    /// <param name="v2">Vector from.</param>
    /// <param name="degrees">Change in angle from v2.</param>
    /// <returns>Returns a new vector with the heading of (v2.heading + degrees) and the magnitude of v1.</returns>
    public static Vector2 FromRotation(this Vector2 v1, Vector2 v2, float degrees) {
        float rotation = Vector2.SignedAngle(Vector2.right, v2);
        return v1.SetHeading(degrees + rotation);
    }

    /// <summary>
    /// Sets a vector equal to another.
    /// </summary>
    /// <param name="v">Original vector.</param>
    /// <param name="newVector">New vector.</param>
    /// <returns>Returns v.Set(newVector.x, newVector.y)</returns>
    public static Vector2 Set(this Vector2 v, Vector2 newVector) {
        v.Set(newVector.x, newVector.y);
        return v;
    }
}
