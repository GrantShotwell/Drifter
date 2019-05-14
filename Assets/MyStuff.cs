using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyStuff;
using MyStuff.GeometryObjects;

/* Templates

- - - - - Basic Editor - - - - -
    
[CustomEditor(typeof(\CLASS\))]
[CanEditMultipleObjects]
public class \CLASS\Editor : Editor {
    \CLASS\ script;
    SerializedProperty
        _var;

    private void OnEnable() {
        script = (\CLASS\)target;
        _var = serializedObject.FindProperty("var");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
            
        /\\/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_var, new GUIContent("Variable"));

        serializedObject.ApplyModifiedProperties();
    }
}

- - - - - Fade Group - - - - -

AnimBool m_ShowFields1;

OnEnable() {
    m_ShowFields1 = new AnimBool(true);
    m_ShowFields1.valueChanged.AddListener(Repaint);
}

OnInspectorGUI() {
    m_ShowFields1.target = script.bool;
    if(EditorGUILayout.BeginFadeGroup(m_ShowFields1.faded)) {
        EditorGUI.indentLevel++;

        /\\/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_var, new GUIContent("Variable"));

        EditorGUI.indentLevel--;
    }
    EditorGUILayout.EndFadeGroup();
}

- - - - -  - - - - -

*/

namespace MyStuff {
    #region Geometry
    /// <summary>
    /// A collection of math functions related to Geometry.
    /// </summary>
    public class Geometry {

        #region Methods

        #region Tests
        /// <summary>
        /// On a 1-Dimentional line, the direction from X1 to X2 is equal to the sign of the input.
        /// </summary>
        /// <param name="input">X1 - X2</param>
        /// <returns>Returns the sign of the input (-1 or +1).</returns>
        public static int LinearDirection(float input) {
            if(input < 0) return -1;
            else return 1;
        }
        
        /// <summary>
        /// On a 1-Dimentional line, the direction from X1 to X2 is equal to the sign of the input.
        /// </summary>
        /// <param name="input">X1 - X2</param>
        /// <returns>Returns the sign of the input (-1 or +1).</returns>
        public static Direction LinearDirection(float input, Axis axis) {
            if(input < 0) {
                if(axis == Axis.Horizontal) return Direction.Down;
                else return Direction.Left;
            }
            else {
                if(axis == Axis.Horizontal) return Direction.Up;
                else return Direction.Right;
            }
        }

        /// <summary>
        /// Gives which direction the vector is more facing on the given axis.
        /// </summary>
        /// <param name="vector">The vector to compare with the given axis.</param>
        /// <param name="axis">The axis to compare the given vector to.</param>
        /// <returns>Returns the sign of the input (-1 or +1).</returns>
        public static Direction LinearDirection(Vector2 vector, Axis axis) {
            if(axis == Axis.Horizontal)
                return LinearDirection(vector.x, axis);
            else
                return LinearDirection(vector.y, axis);
        }
        
        /// <summary>
        /// Gives the Geometry.Direction of an angle relative to the given Geometry.Axis.
        /// </summary>
        /// <param name="angle">The input angle in degrees.</param>
        /// <param name="axis">The axis to make the output direction relative to.</param>
        /// <returns>Axis.Horizontal relates to Quadrant 1 and 2 for positive. Axis.Vertical relates to Quadrant 1 and 4 for positive.</returns>
        public static Direction AngleDirection(Angle angle, Axis axis = Axis.Horizontal) {
            float theta = angle.degrees;
            if(axis == Axis.Horizontal) {
                if(theta == -90) return Direction.Right;
                if(theta == 90) return Direction.Left;
                if(-90 > angle && angle > 90) return Direction.Right;
                return Direction.Left;
            }
            else {
                if(theta == 0) return Direction.Right;
                if(theta == 180) return Direction.Left;
                if(0 > angle && angle > 180) return Direction.Up;
                else return Direction.Down;
            }
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
        /// Determines if the given value exists.
        /// </summary>
        /// <returns>Returns 'true' if the number is not Infinity and is not NaN.</returns>
        public static bool Exists(float number) { return (!float.IsInfinity(number)) && (!float.IsNaN(number)); }

        /// <summary>
        /// Determines if the given value exists.
        /// </summary>
        /// <returns>Returns 'true' if the components are not Infinity nor NaN.</returns>
        public static bool Exists(Vector2 vector) { return Exists(vector.x) && Exists(vector.y); }

        /// <summary>
        /// Checks if the input is on the interval [-range, +range].
        /// </summary>
        /// <param name="range">[-range, +range]</param>
        /// <returns>Returns true if input is on the interval [-range, +range]</returns>
        public static bool IsBetweenRange(float input, float range) {
            return -range <= input && input <= range;
        }
        #endregion

        #region Conversions

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

        //todo: replace the 'while()' with math
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

        //todo: replace the 'while()' with math
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
        #endregion

        #region Finders
        /// <summary>
        /// Returns the angle between v1 and v2 with respect to the X/Y axies.
        /// </summary>
        /// <param name="v1">Vector 1 (angle from)</param>
        /// <param name="v2">Vector 2 (angle to)</param>
        public static Angle GetAngle(Vector2 v1, Vector2 v2) {
            return Mathf.Atan2(v1.y - v2.y, v1.x - v2.x);
        }

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
        public static Line LineFromAngle(Vector2 point, Angle angle) {
            float a = Mathf.Tan(angle.radians);
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
        /// Creates a new Vector2 with the given angle and magnitude.
        /// </summary>
        /// <param name="angle">The angle of the vector. Specify degrees or radians with 'units'.</param>
        /// <param name="magnitude">The magnitude of the vector.</param>
        /// <param name="units">Is the angle in degrees or radians?</param>
        /// <returns>Returns 'new Vector2(Cos(radians) * magnitude, Sin(radians) * magnitude)'</returns>
        public static Vector2 Vector2FromAngle(Angle angle, float magnitude) {
            float theta = angle.radians;
            return new Vector2(
                Mathf.Cos(theta) * magnitude,
                Mathf.Sin(theta) * magnitude
            );
        }

        /// <summary>
        /// Creates a new Vector2 with the given angle and a magnitude of 1.
        /// </summary>
        /// <param name="angle">The angle of the vector. Specify degrees or radians with 'units'.</param>
        /// <param name="units">Is the angle in degrees or radians?</param>
        /// <returns>Returns 'new Vector2(Cos(radians) * magnitude, Sin(radians) * magnitude)'</returns>
        public static Vector2 Vector2FromAngle(Angle angle) {
            float theta = angle.radians;
            return new Vector2(
                Mathf.Cos(theta),
                Mathf.Sin(theta)
            );
        }

        public static Vector2 VectorDirection(Direction direction) {
            /**/ if(direction == Direction.Right)  return Vector2.right;
            else if(direction == Direction.Left)   return Vector2.left;
            else if(direction == Direction.Up)     return Vector2.up;
            else if(direction == Direction.Down)   return Vector2.down;
            else return Vector2.zero;
        }

        /// <summary>
        /// Finds the angle between v1 and v2 if v2's tail is placed on the head of v1.
        /// This angle can also be described as finding the angle of a bend in a line.
        /// </summary>
        /// <param name="v1">The first vector in the angle.</param>
        /// <param name="v2">The second vector in the angle.</param>
        public static Angle HeadToTailAngle(Vector2 v1, Vector2 v2) {
            return Angle.radLeft - v1.Heading().value + v2.Heading().value;
        }

        /// <summary>
        /// Finds the shortest vector that goes from the given point to the given line.
        /// </summary>
        /// <param name="point">The point where to find the path from.</param>
        /// <param name="line">The line to find the path to.</param>
        public static Vector2 PathToLine(Vector2 point, Line line) {
            Line towards = LineFromAngle(point, line.Angle().Mirror());
            Vector2 intersection = Intersection(line, towards);
            Vector2 direction = intersection - point;
            return direction;
        }
        #endregion

        #region Setters
        /// <summary>
        /// Limits the input to [+/-] limit.
        /// </summary>
        /// <param name="input">The input value.</param>
        /// <param name="limit">The limiting value.</param>
        public static float LimitTo(float input, float limit) {
            if(Mathf.Abs(input) > Mathf.Abs(limit)) input = limit * LinearDirection(input);
            return input;
        }

        /// <summary>
        /// Changes the heading of v2 so that if v2's tail is placed on the head of v1, the angle between those two vectors is 'degrees'.
        /// This can also be described as setting the angle of a bend in a line.
        /// </summary>
        /// <param name="v1">Unchanged vector.</param>
        /// <param name="v2">Changed vector.</param>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>Returns v2.SetHeading(180.0f - v1.Heading() - degrees).</returns>
        public static Vector2 HeadToTailAngle(Vector2 v1, Vector2 v2, Angle angle) {
            return v2.SetHeading(Angle.radLeft - v1.Heading().value - angle.radians);
        }
        #endregion

        #region Intersections
        /// <summary>
        /// Finds the intersection between two or more Geometry objects.
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
        /// Finds the intersections between two or more Geometry objects.
        /// </summary>
        /// <param name="c1">Circle 1</param>
        /// <param name="c2">Circle 2</param>
        /// <returns>Returns [the closest] two intersections between the two circles.</returns>
        public static Vector2[] Intersection(Circle c1, Circle c2) {
            float r1 = c1.radius, r2 = c2.radius;
            float d = Vector2.Distance(c1.center, c2.center);
            if(d > c1.radius + c2.radius) {
                Vector2[] i = {   // only gets here if there is no real intersection
                    Vector2.Lerp(c1.center, c2.center, c1.radius / d),
                    Vector2.Lerp(c1.center, c2.center, c2.radius / d)
                };
                return i;
            }

            // squared versions of the variables, because we use them a lot.
            float d_2 = d * d, r1_2 = r1 * r1, r2_2 = r2 * r2;

            float b = d_2 - r1_2 + r2_2;
            float x = b / (2 * d);
            float a = (1 / d) * Mathf.Sqrt((4 * d_2 * r2_2) - (b * b));
            float y = (a / 2);

            float angle = GetAngle(c1.center, c2.center);

            Vector2[] intersections = new Vector2[2];
            intersections[0] = new Vector2(x, +y).Rotate(angle) + c1.center;
            intersections[1] = new Vector2(x, -y).Rotate(angle) + c1.center;

            return intersections;
        }

        /// <summary>
        /// Finds the intersections between two or more Geometry objects.
        /// </summary>
        /// <param name="c1">Circle 1.</param>
        /// <param name="c2">Circle 2.</param>
        /// <param name="c3">Circle 3.</param>
        /// <returns>Returns [the closest] intersection shared by all three circles.</returns>
        public static Vector2 Intersection(Circle c1, Circle c2, Circle c3) {
            var i1 = Intersection(c1, c2);
            var i2 = Intersection(c1, c3);

            int smallest = 0;
            float[] D = new float[4];
            D[0] = Vector2.Distance(i1[0], i2[0]);
            D[1] = Vector2.Distance(i1[0], i2[1]);
            D[2] = Vector2.Distance(i1[1], i2[0]);
            D[3] = Vector2.Distance(i1[1], i2[1]);

            for(int j = 1; j < 4; j++)
                if(D[smallest] > D[j]) smallest = j;

            return i2[smallest % 2]; //not 100% sure on this part, might be i1 instead?
        }
        #endregion
        
        #endregion

    }

    namespace GeometryObjects {

        #region Enumerators
        /// <summary>General direction in terms of left/right/up/down. More specifically, the normalized direction on either the X or Y axis.</summary>
        public enum Direction { Up, Down, Right, Left }

        /// <summary>Positive = 1, Negative = -1.</summary>
        public enum Sign { Negative = -1, Zero = 0, Positive = 1 }

        /// <summary>x : y : : horizontal : vertical</summary>
        public enum Axis { Horizontal, Vertical }

        /// <summary>Enumerator for radians or degrees.</summary>
        public enum AngleType { Radians, Degrees }
        #endregion

        #region Classes
        /// <summary>
        /// Ties together a value and a unit for an angle. Often used by the Geometry class.
        /// </summary>
        public struct Angle {

            /// <summary>The value of the angle with unknown units.</summary>
            public float value;

            /// <summary>The units of the angle.</summary>
            public AngleType type;

            /// <summary>
            /// Creates a new angle
            /// </summary>
            /// <param name="value">The value of the angle in the units given.</param>
            /// <param name="type">The units of the angle.</param>
            public Angle(float value, AngleType type = AngleType.Radians) {
                this.value = value;
                this.type = type;
            }

            public Angle(Angle original) {
                value = original.value;
                type = original.type;
            }

            /// <summary>
            /// Changes the units and applies the conversion.
            /// </summary>
            /// <param name="type">The new units of the angle.</param>
            public Angle ConvertType(AngleType type) {
                if(this.type != type) {
                    /**/ if(type == AngleType.Degrees)
                        value *= Mathf.Rad2Deg;
                    else if(type == AngleType.Radians)
                        value *= Mathf.Deg2Rad;
                    this.type = type;
                }
                return this;
            }

            /// <summary>Use this only if you don't know the units for sure. If you do, then use 'Angle.value'.</summary>
            public float radians =>
                type == AngleType.Radians ? value
                : value * Mathf.Deg2Rad;

            /// <summary>Use this only if you don't know the units for sure. If you do, then use 'Angle.value'.</summary>
            public float degrees =>
                type == AngleType.Degrees ? value
                : value * Mathf.Rad2Deg;

            public Angle Normalize() {
                float circle = type == AngleType.Degrees ? degRight : radRight;
                value = value % circle;
                return this;
            }

            public Angle Center() {
                float circle = type == AngleType.Degrees ? degRight : radRight;
                value = value % circle;
                float halfCircle = circle / 2f;
                if(value > halfCircle) value -= circle;
                if(value < halfCircle) value += circle;
                return this;
            }

            public Angle Mirror() {
                value += type == AngleType.Degrees ? degUp : radUp;
                return this;
            }

            public Angle Abs() {
                if(value < 0) return new Angle(-value, type);
                else return new Angle(this);
            }
            
            /// <summary>Casts a float into a new Angle with units of radians.</summary>
            public static implicit operator Angle(float value) { return new Angle(value, AngleType.Radians); }
            /// <summary>Casts an angle into a float with 'Angle.radians'.</summary>
            public static implicit operator float(Angle angle) { return angle.radians; }
            /// <summary>Casts an int into a new Angle with units of degrees.</summary>
            public static implicit operator Angle(int value) { return new Angle(value, AngleType.Degrees); }
            /// <summary>Casts an angle into an int with 'Angle.degrees'.</summary>
            public static explicit operator int(Angle angle) { return (int)angle.degrees; }
            
            public static Angle operator +(Angle left, Angle right) { return new Angle(left.radians + right.radians, AngleType.Radians).ConvertType(left.type); }
            public static Angle operator -(Angle left, Angle right) { return new Angle(left.radians - right.radians, AngleType.Radians).ConvertType(left.type); }
            public static Angle operator *(Angle left, Angle right) { return new Angle(left.radians * right.radians, AngleType.Radians).ConvertType(left.type); }
            public static Angle operator /(Angle left, Angle right) { return new Angle(left.radians / right.radians, AngleType.Radians).ConvertType(left.type); }
            public static Angle operator %(Angle left, Angle right) { return new Angle(left.radians % right.radians, AngleType.Radians).ConvertType(left.type); }
            public static bool operator >(Angle left, Angle right) { return left.radians > right.radians; }
            public static bool operator <(Angle left, Angle right) { return left.radians < right.radians; }


            public override string ToString() {
                return value + (
                    type == AngleType.Degrees ?
                    " degrees" : " radians"
                );
            }


            /// <summary>1/2 PI</summary>
            public static float radUp = Mathf.PI / 2f;
            /// <summary>1 PI</summary>
            public static float radLeft = Mathf.PI;
            /// <summary>2/3 PI</summary>
            public static float radDown = 2f * Mathf.PI / 3f;
            /// <summary>2 PI</summary>
            public static float radRight = 2f * Mathf.PI;

            public static float degUp = 90.0f;
            public static float degLeft = 180.0f;
            public static float degDown = 270.0f;
            public static float degRight = 360.0f;

            public static float RadDirection(Direction direction) {
                switch(direction) {
                    case Direction.Up:     return radUp;
                    case Direction.Left:   return radLeft;
                    case Direction.Down:   return radDown;
                    case Direction.Right:  return radRight;
                    default: return 0f;
                }
            }

            public static float DegDirection(Direction direction) {
                switch(direction) {
                    case Direction.Up:     return degUp;
                    case Direction.Left:   return degLeft;
                    case Direction.Down:   return degDown;
                    case Direction.Right:  return degRight;
                    default: return 0f;
                }
            }
        }

        /// <summary>
        /// Stores data and methods for a line. Often used by the Geometry class.
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

            /// <summary>The slope of the line.</summary>
            public float slope => -a;

            /// <summary>Angle of the slope.</summary>
            public Angle Angle(AngleType units = AngleType.Degrees) {
                if(units == AngleType.Radians) return new Angle(Mathf.Atan(slope), units);
                else return new Angle(Mathf.Atan(slope) * Mathf.Rad2Deg, units);
            }

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
                float theta = Angle(AngleType.Radians) * direction;
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
        /// Stores data and methods for a triangle. Often used by the Geometry class.
        /// </summary>
        public struct Triangle {
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
                A = Geometry.LawOfCosForAngleA(a, b, c);
                B = Geometry.LawOfCosForAngleB(a, b, c);
                C = Geometry.LawOfCosForAngleC(a, b, c);
            }

            public void SolveForAngles() {
                A = Geometry.LawOfCosForAngleA(a, b, c);
                B = Geometry.LawOfCosForAngleB(a, b, c);
                C = Geometry.LawOfCosForAngleC(a, b, c);
            }

            public override string ToString() {
                return base.ToString();
            }
        }

        /// <summary>
        /// Stores data and methods for a circle. Often used by the Geometry class.
        /// </summary>
        public struct Circle {
            public Vector2 center;
            public float radius;
            public Circle(Vector2 center, float radius) {
                this.center = center;
                this.radius = radius;
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
    public struct Range {
        /// <summary>Unlimited range.</summary>
        public static Range infinite = new Range(Mathf.NegativeInfinity, Mathf.Infinity);

        /// <summary>Area of 1: (-0.5, +0.5)</summary>
        public static Range half = new Range(-0.5f, 0.5f);

        public float size   => max - min;
        public float offset => min + (size / 2);
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

        /// <summary>
        /// Places the given value to either the minimum or maximum: whichever is closer.
        /// </summary>
        /// <returns>Returns the closest min/max to the given value.</returns>
        public float PlaceOutside(float value) {
            if(Contains(value)) return value;
            if((min - value).Abs() > (max - value).Abs())
                return min;
            return max;
        }

        public float Random() {
            return UnityEngine.Random.Range(min, max);
        }

        public float RandomSpread() {
            float n = UnityEngine.Random.value * UnityEngine.Random.Range(-1.0f, +1.0f) / 2 + 0.5f;
            return n * (max - min) + min;
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
        public static Range operator +(Range left, Range right) { return new Range(left.min + right.min, left.max + right.max); }
        public static Range operator -(Range left, Range right) { return new Range(left.min - right.min, left.max - right.max); }
        public static Range operator *(Range left, Range right) { return new Range(left.min * right.min, left.max * right.max); }
        public static Range operator /(Range left, Range right) { return new Range(left.min / right.min, left.max / right.max); }
        public static Range operator %(Range left, Range right) { return new Range(left.min % right.min, left.max % right.max); }
        public static bool operator ==(Range left, Range right) { return left.min == right.min && left.max == right.max; }
        public static bool operator !=(Range left, Range right) { return left.min != right.min && left.max != right.max; }
        
        public static explicit operator Vector2(Range range) { return new Vector2(range.min, range.max); }

        #region Stuff the compiler generated and will complain if it's not here.
        public override bool Equals(object obj) {
            if(!(obj is Range)) {
                return false;
            }

            var range = (Range)obj;
            return min == range.min &&
                   max == range.max;
        }
        public override int GetHashCode() {
            var hashCode = -897720056;
            hashCode = hashCode * -1521134295 + min.GetHashCode();
            hashCode = hashCode * -1521134295 + max.GetHashCode();
            return hashCode;
        }
        #endregion
        #endregion
    }

    /// <summary>
    /// Class for an inclusive Vector2 range for x and y components.
    /// </summary>
    [System.Serializable]
    public struct Range2D {
        /// <summary>Unlimited range.</summary>
        public static Range2D infinite = new Range2D(Range.infinite, Range.infinite);

        /// <summary>Area of 1.</summary>
        public static Range2D half = new Range2D(Range.half, Range.half);

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
        /// Creates a new Range2D.
        /// </summary>
        public Range2D(float x_min, float x_max, float y_min, float y_max) {
            x = new Range(x_min, x_max);
            y = new Range(y_min, y_max);
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
            return new Vector2(
                x.Place(value.x),
                y.Place(value.y)
            );
        }

        public Vector2 PlaceOutside(Vector2 value) {
            return new Vector2(
                x.PlaceOutside(value.x),
                y.PlaceOutside(value.y)
            );
        }

        public Vector2 Random() {
            return new Vector2(x.Random(), y.Random());
        }

        public Vector2 RandomSpread() {
            return new Vector2(x.RandomSpread(), y.RandomSpread());
        }

        public override string ToString() {
            return "[" + x + ", " + y + "]";
        }
    }

    /*AngleRange
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
    */
    #endregion

    #region Camera Control
    /// <summary>
    /// Custom PropertyDrawer for CameraLimit.
    /// </summary>
    [CustomPropertyDrawer(typeof(CameraLimit))]
    public class CameraLimitProperty : PropertyDrawer {
        static readonly float space = 2;

        public static string[] paths = {
            "posLim",
            "positionRange",
            "positionVector",
            "size",
            "lerpLimit",
            "lerp"
        };

        public static string[] names = {
            "Position Limit Type",
            "Position Range",
            "Position Vector",
            "Size Limit",
            "Limit the lerp value?",
            "Lerp Limit"
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            if(property.isExpanded) {
                float height = base.GetPropertyHeight(property, label);
                for(int j = 0; j < paths.Length; j++) {
                    SerializedProperty prop = property.FindPropertyRelative(paths[j]);
                    bool show = true;
                    if(j == 1 || j == 2) show = j == property.FindPropertyRelative(paths[0]).enumValueIndex;
                    if(j == 5) show = property.FindPropertyRelative(paths[4]).boolValue;
                    if(show) height += EditorGUI.GetPropertyHeight(prop, new GUIContent(names[j]), true) + space;
                }
                return height;
            }
            else return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            position = AddProperty(position, property, true);
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);

            if(property.isExpanded) {
                float indent = 15;
                position.x += indent;
                position.width -= indent;
                
                SerializedProperty _posLim = property.FindPropertyRelative("posLim");
                position = AddProperty(position, _posLim);
                EditorGUI.PropertyField(position, _posLim, new GUIContent("Type of limit on the position."));

                if(_posLim.enumValueIndex == 1) {
                    SerializedProperty _posRange = property.FindPropertyRelative("positionRange");
                    position = AddProperty(position, _posRange);
                    EditorGUI.PropertyField(position, _posRange, new GUIContent("Position Range"), true);
                }

                if(_posLim.enumValueIndex == 2) {
                    SerializedProperty _posVector = property.FindPropertyRelative("positionVector");
                    position = AddProperty(position, _posVector);
                    EditorGUI.PropertyField(position, _posVector, new GUIContent("Position Vector"));
                }
                
                SerializedProperty _size = property.FindPropertyRelative("size");
                position = AddProperty(position, _size);
                EditorGUI.PropertyField(position, _size, new GUIContent("Size Range"), true);

                SerializedProperty _lerpLimit = property.FindPropertyRelative("lerpLimit");
                position = AddProperty(position, _lerpLimit);
                EditorGUI.PropertyField(position, _lerpLimit, new GUIContent("Set a value for camera lerp?"));

                if(_lerpLimit.boolValue) {
                    SerializedProperty _lerp = property.FindPropertyRelative("lerp");
                    position = AddProperty(position, _lerp);
                    EditorGUI.PropertyField(position, _lerp, new GUIContent("Lerp Value"));
                }
            }
        }
        
        private Rect AddProperty(Rect position, SerializedProperty property, bool first = false) {
            if(first) {
                position.height = base.GetPropertyHeight(property, new GUIContent(names[0]));
            }
            else {
                position.y += position.height;
                position.height = EditorGUI.GetPropertyHeight(property, new GUIContent(names[0]));
                position.y += space;
            }
            return position;
        }
    }

    /// <summary>
    /// Class for containing data on how to limit the position, size, and lerp of the camera.
    /// </summary>
    [Serializable]
    public class CameraLimit {
        /// <summary>Range, Vector, or None</summary>
        public enum PositionLimit { None = 0, Range = 1, Vector = 2 }

        /// <summary>The type of position limit.</summary>
        public PositionLimit posLim;

        /// <summary>The area the camera is forced to be in.</summary>
        public Range2D positionRange;

        /// <summary>The position the camera is allowed to be in.</summary>
        [VectorPicker] public Vector2 positionVector;

        /// <summary>Does this CameraLimit have a definition for position limit?</summary>
        public bool limitsPosition => posLim != PositionLimit.None;

        /// <summary>The size(s) the camera is allowed to be.</summary>
        public Range size;

        /// <summary>Does this CameraLimit have a definition for size limit?</summary>
        public bool limitsSize => size != Range.infinite;

        /// <summary>Set the lerp amount of the camera?</summary>
        public bool lerpLimit;

        /// <summary>The lerp limit.</summary>
        public float lerp;
    }
    #endregion

    #region Actions
    /// <summary>
    /// Class for actions on a delay.
    /// Meant to be used with 'TimedActionTracker'.
    /// </summary>
    public class TimedAction {
        public Action action;
        public float startTime, finalTime;
        public string tag;

        /// <summary>
        /// Creates a action that will be run after the delay is over.
        /// </summary>
        /// <param name="delay">The amount of time that the action is delayed.</param>
        /// <param name="whenFinished">Action to run when the delay is over.</param>
        public TimedAction(float delay, Action whenFinished) : this(delay, "default", whenFinished) {}

        /// <summary>
        /// Creates an action that will be run after the delay is over.
        /// </summary>
        /// <param name="delay">The amount of time that the action is delayed.</param>
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
    /// Meant to be used with 'TimedActionTracker'.
    /// </summary>
    public class ContinuousAction : TimedAction {
        /// <summary>
        /// Create a ContinuousAction with no delay.
        /// </summary>
        /// <param name="duration">The amount of time the Action is active.</param>
        /// <param name="duringTime">The Action to run while active.</param>
        public ContinuousAction(float duration, Action duringTime) : this(0.0f, duration, duringTime) {}

        /// <summary>
        /// Create a ContinuousAction with a tag and no delay.
        /// </summary>
        /// <param name="duration">The amount of time the Action is active.</param>
        /// <param name="tag">Optional identifier tag. (default = "default")</param>
        /// <param name="duringTime">The Action to run while active.</param>
        public ContinuousAction(float duration, string tag, Action duringTime) : this(0.0f, duration, tag, duringTime) {}

        /// <summary>
        /// Create a ContinuousAction with a delay.
        /// </summary>
        /// <param name="delay">The amount of time that the Action is delayed.</param>
        /// <param name="duration">The amount of time the Action is active.</param>
        /// <param name="duringTime">The Action to run while active.</param>
        public ContinuousAction(float delay, float duration, Action duringTime) : this(delay + duration, duration, "default", duringTime) { }

        /// <summary>
        /// Create a ContinuousAction with a delay and tag.
        /// </summary>
        /// <param name="delay">The amount of time that the Action is delayed.</param>
        /// <param name="duration">The amount of time the Action is active.</param>
        /// <param name="tag">Optional identifier tag. (default = "default")</param>
        /// <param name="duringTime">The Action to run while active.</param>
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
        /// Keeps track of, and invokes, TimedActions by calling the Update() function from your code.
        /// </summary>
        /// <param name="deleteWhenDone">Delete the TimedAction from the list when finished?</param>
        public TimedActionTracker(bool deleteWhenDone = true) {
            singleUse = deleteWhenDone;
        }

        /// <summary>
        /// Checks if any of the TimedActions are ready to be called.
        /// Should be usually called in MonoBehaviour.Update() or MonoBehaviour.FixedUpdate().
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
        /// <returns>Returns 'true' if at least one item was removed.</returns>
        public bool RemoveTagged(string tag) {
            bool removed = false;
            foreach(TimedAction action in timedActions) {
                if(action.tag == tag) {
                    timedActions.Remove(action);
                    removed = true;
                }
            }
            return removed;
        }

        /// <summary>
        /// Remove a specific TimedAction from the list.
        /// </summary>
        /// <param name="action">TimedAction to remove from the list.</param>
        /// <returns>Returns 'true' if a TimedAction was removed.</returns>
        public bool RemoveAction(TimedAction action) { return timedActions.Remove(action); }

        /// <summary>
        /// Remove a specific TimedAction from the list by Action.
        /// </summary>
        /// <param name="action">A TimedAction with this Action will be removed if found.</param>
        /// <returns>Returns 'true' if a TimedAction was removed.</returns>
        public bool RemoveAction(Action action) {
            foreach(TimedAction timedAction in timedActions)
                if(timedAction.action == action)
                    return timedActions.Remove(timedAction);
            return false;
        }
    }
    #endregion

    #region Debug
    public static class ExtendedDebug {
        public static void DrawPinwheel(Vector2 origin, Angle turn, Color color, float displacement = 0.0f) {
            Range range = new Range(0, 360) + displacement;
            Angle angle = new Angle(range.min, AngleType.Degrees);
            Vector2 direction = Vector2.right.Rotate(angle);
            while(range.Contains(angle.degrees)) {
                Debug.DrawRay(origin, direction, color);
                direction = direction.Rotate(turn);
                angle += turn;
            }
        }
        
        public static void BoxCast2D(Vector2 origin, RaycastHit2D hit) { BoxCast2D(origin, hit, Color.white); }
        public static void BoxCast2D(Vector2 origin, RaycastHit2D hit, Color color) {
            Debug.DrawLine(origin, hit.point, color);
        }
    }
    #endregion

    #region Other Classes
    /// <summary>
    /// Stores data about linear and angular velocity to give to a new instantiated object.
    /// </summary>
    [System.Serializable]
    public struct Movement {
        /// <summary>A range of possible linear velocities.</summary>
        public Range2D linearRange;
        /// <summary>A range of possible angular velocities.</summary>
        public Range angularRange;

        public Movement(Range2D linear, Range angular) {
            linearRange = linear;
            angularRange = angular;
        }

        /// <summary>
        /// Finds a linear velocity.
        /// </summary>
        /// <returns>Returns a random Vector2 that is inside the Range2D linearRange.</returns>
        public Vector2 GetLinear() {
            return linearRange.Random();
        }

        /// <summary>
        /// Finds a linear velocity.
        /// </summary>
        /// <param name="spread">Use Range2D.RandomSpread()?</param>
        /// <returns>Returns a random Vector2 that is inside the Range2D linearRange.</returns>
        public Vector2 GetLinear(bool spread) {
            if(spread) return linearRange.RandomSpread();
            else return linearRange.Random();
        }

        /// <summary>
        /// Finds an angular velocity.
        /// </summary>
        /// <returns>Returns a random float that is inside the Range angularRange.</returns>
        public float GetAngular() {
            return angularRange.Random();
        }

        /// <summary>
        /// Finds an angular velocity.
        /// </summary>
        /// <param name="spread">Use Range.RandomSpread()?</param>
        /// <returns>Returns a random float that is inside the Range angularRange.</returns>
        public float GetAngular(bool spread) {
            if(spread) return angularRange.RandomSpread();
            else return angularRange.Random();
        }

        public void Set(GameObject gameObject, bool spread = true) {
            Set(gameObject.GetComponent<Rigidbody2D>(), spread);
        }
        public void Set(Rigidbody2D rigidbody, bool spread = true) {
            rigidbody.velocity = GetLinear(spread);
            rigidbody.angularVelocity = GetAngular(spread);
        }

        public void Add(GameObject gameObject, bool spread = true) {
            Add(gameObject.GetComponent<Rigidbody2D>(), spread);
        }
        public void Add(Rigidbody2D rigidbody, bool spread = true) {
            rigidbody.velocity += GetLinear(spread);
            rigidbody.angularVelocity += GetAngular(spread);
        }
    }

    /// <summary>
    /// A float array that is efficient in calculating an average for data that is always changing.
    /// </summary>
    [System.Serializable]
    public class RollingFloatArray {
        private float[] array;
        private int index = 0;

        public float sum { get; private set; }
        public float average => sum / array.Length;

        public RollingFloatArray(int size) { array = new float[size]; }

        public void Add(float number) {
            sum -= array[index];
            sum += number;
            array[index] = number;
            index = array.Length % ++index;
        }
    }

    /// <summary>
    /// A UnityEngine.Object array. Add(T) will remove the oldest item if the size has been reached.
    /// </summary>
    /// <typeparam name="T">Type of the array.</typeparam>
    public class RollingArray<T> {
        private readonly T[] array;
        private int offset = 0;
        public readonly int size;
        private readonly Action<T> destroy;

        public RollingArray(int size, Action<T> destroy) {
            array = new T[size];
            this.size = size;
            this.destroy = destroy;
        }

        public T Get(int index) {
            if(index + offset >= size)
                index -= size;
            return array[index];
        }

        public void Add(T item) {
            offset++;
            if(offset >= size)
                offset -= size;
            if(array[offset] != null)
                Destroy(array[offset]);
            array[offset] = item;
        }

        public void Destroy(T item) { destroy(item); }
        public void Destroy(int index) { destroy(Get(index)); }
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
    
    #region Enumerators
    /// <summary>
    /// Update, FixedUpdate, or None
    /// </summary>
    public enum UpdateLocation { Update, FixedUpdate, None }
	#endregion

    #region Extensions
    public static class Extensions {
        #region Vector2
        /// <summary>
        /// The heading of the Vector. Default units of the angle are radians.
        /// </summary>
        /// <returns>Returns the degrees of this vector from Vector2.right.</returns>
        public static Angle Heading(this Vector2 v) { return Mathf.Atan2(v.y, v.x); }

        /// <summary>
        /// Sets the heading of the vector.
        /// </summary>
        /// <param name="v">Original vector.</param>
        /// <param name="angle">Amount of rotation in degrees.</param>
        /// <returns>Returns a new vector is a heading of v.Heading + degrees and magnitude of the original.</returns>
        public static Vector2 SetHeading(this Vector2 v, Angle angle)      { return new Vector2(v.magnitude, 0).Rotate(angle); }

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
        /// <param name="angle">The rotation angle.</param>
        /// <returns>Returns a vector of the same magnitude, but rotated.</returns>
        public static Vector2 Rotate(this Vector2 vector, Angle angle) {
            float theta = angle.radians;
            float sin = Mathf.Sin(theta), cos = Mathf.Cos(theta);
            float x = vector.x, y = vector.y;
            vector.x = (cos * x) - (sin * y);
            vector.y = (sin * x) + (cos * y);
            return vector;
        }

        /// <summary>
        /// Rotates v1 towards v2.
        /// </summary>
        /// <param name="v1">Original vector.</param>
        /// <param name="v2">Vector to.</param>
        /// <param name="angle">Amount of rotation.</param>
        /// <returns>Returns v1.Rotate([+/-]degrees).</returns>
        public static Vector2 RotateTo(this Vector2 v1, Vector2 v2, Angle angle) {
            float theta = angle.radians;
            if(Vector2.SignedAngle(v1, v2) > 0) return Rotate(v1, theta);
            else return Rotate(v1, -theta);
        }

        /// <summary>
        /// Rotates v1 away from v2.
        /// </summary>
        /// <param name="v1">Original vector.</param>
        /// <param name="v2">Vector from.</param>
        /// <param name="theta">Amount of rotation.</param>
        /// <returns>Returns v1.Rotate([-/+]degrees).</returns>
        public static Vector2 RotateFrom(this Vector2 v1, Vector2 v2, Angle angle) {
            float theta = angle.radians;
            if(Vector2.SignedAngle(v1, v2) < 0) return Rotate(v1, theta);
            else return Rotate(v1, -theta);
        }

        /// <summary>
        /// Sets the heading of v1 equal to the heading of v2, then rotates it.
        /// </summary>
        /// <param name="v1">Original vector.</param>
        /// <param name="v2">Vector from.</param>
        /// <param name="angle">Change in angle from v2.</param>
        /// <returns>Returns a new vector with the heading of (v2.heading + degrees) and the magnitude of v1.</returns>
        public static Vector2 FromRotation(this Vector2 v1, Vector2 v2, Angle angle) {
            float theta = angle.radians;
            float rotation = Vector2.SignedAngle(Vector2.right, v2);
            return v1.SetHeading(theta + rotation);
        }
        #endregion

        #region primitives
        /// <summary>
        /// Mathf.Abs() but specifically for making numbers positive.
        /// </summary>
        public static float Abs(this float n) {
            if(n < 0) return -n;
            else return n;
        }

        /// <summary>
        /// Mathf.Abs() but specifically for making numbers positive.
        /// </summary>
        public static int Abs(this int n) {
            if(n < 0) return -n;
            else return n;
        }
        #endregion

        #region Misc.
        public static string String(this char[] chars) {
            string s = string.Empty;
            foreach(char c in chars) s += c;
            return s;
        }

        public static string String(this List<char> chars) {
            string s = string.Empty;
            foreach(char c in chars) s += c;
            return s;
        }
        #endregion
    }
    #endregion
}
