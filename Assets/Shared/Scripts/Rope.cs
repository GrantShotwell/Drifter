using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;
using MyStuff.GeometryObjects;
using System;

public class Rope : MonoBehaviour {

    #region Variables
    public LineQueue lineQueue;
    public Color color;
    public float minimumLength = 0.1f;
    public bool attached => Count > 0;

    new Rigidbody2D rigidbody;
    #endregion

    #region Update
    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    #region Internal(ish) Variables
    #region Wrap/Unwrap
    static internal float error1 = 0.30f;
    static internal float error2 = 0.05f;
    static internal float error3 = error1 + error2;
    internal List<Vector2> points = new List<Vector2>();
    internal List<Collider2D> colliders = new List<Collider2D>();
    internal int _Count = 0;
    internal int Count {
        get { return _Count; }
        set {
            _Count = value;
            point1Index = value - 2;
            point2Index = value - 1;
        }
    }
    internal bool foundEdge = false;
    internal float dot;
    internal Angle angle;
    internal Vector2
        normal = Vector2.zero,
        intersection = Vector2.zero,
        path = Vector2.zero,
        pathNrml = Vector2.zero,
        backwards = Vector2.zero,
        forwards = Vector2.zero;
    internal int point1Index = 0, point2Index = 0;
    internal Vector2 Point1 => points[point1Index];
    internal Vector2 Point2 => points[point2Index];
    internal Vector2 Segment1 => Point2 - Point1;
    internal Vector2 Segment2 => Position - Point2;
    internal Vector2 Position => transform.position;
    Vector2[] debugs;
    #endregion
    #region Spiderman Physics
    public Vector2 swingpoint { get; private set; }
    public bool connected => points.Count > 0;
    public float length { get; private set; } = 0;
    public float distance { get; private set; } = 0;
    public bool hasTension { get; private set; }
    #endregion
    #endregion

    private void FixedUpdate() {

        #region Wrap/Unwrap
        //Unwrap
        if(Count > 1) {

            if(!foundEdge) FindEdge();
            else DoUnwrapGeometry();

            lengthUpdate.Invoke(Count - 1);

            //Determine if the edge is still 'holding onto' the rope.
            dot = Vector2.Dot(normal, pathNrml);
            angle = Geometry.HeadToTailAngle(Segment1, Segment2);
            if(
                angle.Abs() > 170
                   &&   dot > 0
            ) {
                Unwrap();
            }

            //Debug
        }

        //Wrap
        if(Count > 0) {
            Vector2 toEndpoint = Point2 - Position;
            RaycastHit2D hit = Physics2D.Raycast(Position, toEndpoint, toEndpoint.magnitude - error1);
            if(hit) Wrap(hit.point, hit.collider);
        }
        #endregion

        #region Spiderman Physics
        if(connected) {
            distance = Vector2.Distance(Position, swingpoint);
            hasTension = (distance > length);

            if(hasTension) {
                #region G _perpendicular (Gp)
                //solving for the perpendicular component of gravity relative to the endpoint.

                float G = Physics2D.gravity.magnitude;
                float ThetaGE = Vector2.SignedAngle(
                    swingpoint - (Vector2)transform.position,
                    Physics2D.gravity
                );
                float ThetaGR = Vector2.SignedAngle(
                    Vector2.right,
                    Physics2D.gravity
                );
                float ThetaGpR = ThetaGR - ThetaGE + 90;

                ThetaGE *= Mathf.Deg2Rad; ThetaGpR *= Mathf.Deg2Rad;
                float Gpx = G * Mathf.Sin(ThetaGE) * Mathf.Cos(ThetaGpR);
                float Gpy = G * Mathf.Sin(ThetaGE) * Mathf.Sin(ThetaGpR);
                Vector2 Gp = new Vector2(Gpx, Gpy);
                #endregion

                Vector2 p = Vector2.Lerp(Position, swingpoint, (distance - length) / distance);
                Vector2 v = TangentVelocity(gameObject, swingpoint);
                v *= ((distance - length) / length) + 1.0f;
                v += Gp * Time.fixedDeltaTime;

                rigidbody.MovePosition((v * Time.fixedDeltaTime) + p);
                rigidbody.velocity = v;

                distance = length;
            }
        }
        else hasTension = false;
        #endregion

    }

    private void OnCollisionStay2D(Collision2D collision) {
        if(hasTension)
            if(rigidbody.velocity.magnitude > 1)
                rigidbody.velocity = rigidbody.velocity.normalized;
    }

    #region Visuals
    private void LateUpdate() {
        //Rope display.
        if(Count > 0) {
            for(int i = 0; i < Count - 1; i++) {
                Vector2 start = points[i];
                Vector2 end = points[i + 1];
                lineQueue.NewLine(start, end, color);
            }
            lineQueue.NewLineFromPlayer(Position, Point2, color);
        }
    }
    private void OnDrawGizmosSelected() {
        if(Count > 1) {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(Point2, pathNrml + Point2);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Point2, normal + Point2);
            
            if(debugs.Length > 5) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(debugs[0], debugs[1]);
                Gizmos.DrawLine(debugs[2], debugs[3]);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(Point2, debugs[4]);
                Gizmos.DrawLine(Point2, debugs[5]);
            }
        }
    }
    #endregion

    #endregion

    #region Methods

    #region Wrap/Unwrap
    internal void FindEdge() {

        Vector2 pointBefore, pointAfter;
        int failsafe = 100; //max steps
        do {

            pointBefore = Point2;
            
            //Slide!
            DoUnwrapGeometry();
            FindEdgeNormal();
            points[point2Index] = colliders[point2Index].ClosestPoint(backwards + Point2);

            pointAfter = Point2;

        } while(Vector2.Distance(pointBefore, pointAfter) > error2 || failsafe --> 0);
        if(failsafe >= 1) Debug.LogWarning("failsafe reached 0");

        foundEdge = true;
    }

    internal void DoUnwrapGeometry() {

        //Find vector (direction) to the line (between) which is between point1 and position.
        Line between = Geometry.LineFromTwoPoints(Point1, Position);
        path = Geometry.PathToLine(Point2, between);
        pathNrml = path.normalized;
        backwards = pathNrml * -error1;
        forwards = pathNrml * error2;

    }

    internal void FindEdgeNormal() {

        //Average the normal from two edges found via raycasting.
        var start1 = backwards.Rotate(+45) + Point2;
        var end1 = forwards + Point2;
        RaycastHit2D hit1 = Physics2D.Raycast(start1, end1 - start1);
        var start2 = backwards.Rotate(-45) + Point2;
        var end2 = forwards + Point2;
        RaycastHit2D hit2 = Physics2D.Raycast(start2, end2 - start2);
        normal = Vector2.Lerp(hit1.normal, hit2.normal, 0.5f).normalized;
        
        debugs = new Vector2[6] { start1, end1, start2, end2, hit1.normal + Point2, hit2.normal + Point2 };
    }

    internal Action<int> lengthUpdate = (int maxIndex) => {};
    internal Action<int> FindLength => (int maxIndex) => {
        length = Vector2.Distance(Position, points[maxIndex]);
        lengthUpdate = (int i) => { };
    };

    internal void Wrap(Vector2 point, Collider2D collider) {
        
        //Add point.
        PointsAdd(point, collider);

        //Change swingpoint.
        swingpoint = Point2;

        //Update length after FindEdge() calls.
        lengthUpdate = FindLength;

    }

    internal void Unwrap() {

        //Remove point.
        PointsRemove();

        //Change swingpoint.
        if(Count > 1) swingpoint = Point2;

        //Update length after FindEdge() calls.
        lengthUpdate = FindLength;

    }
    #endregion

    #region Spiderman Physics
    internal static Vector2 TangentVelocity(GameObject gameObj, Vector2 endpoint) {
        //return the lerped velocity so that it is tangent to the circle (the circle being one with a center at 'swingpoint')

        Rigidbody2D rigidbody = gameObj.GetComponent<Rigidbody2D>();
        Transform transform = gameObj.transform;
        Vector2 newVelocity = rigidbody.velocity;

        Vector2 adjustedVelocity = rigidbody.velocity * Time.fixedDeltaTime;
        Vector2 relativeVelocity = adjustedVelocity + (Vector2)transform.position;
        Line l1 = Geometry.LineFromTwoPoints(relativeVelocity, endpoint);
        Line l2 = Geometry.LineFromAngle(transform.position, Geometry.GetAngle(endpoint, transform.position) - 90);
        if(!Geometry.AreParallel(l1, l2)) {
            Vector2 pointToLerpTo = Geometry.Intersection(l1, l2) - (Vector2)transform.position;
            if(Geometry.Exists(pointToLerpTo)) {
                newVelocity = pointToLerpTo;
                newVelocity /= Time.fixedDeltaTime;
            }
        }

        return newVelocity;
    }

    public void ShortenRope(float amount) {
        length -= amount;
        if(length < minimumLength) length = minimumLength;
    }

    public void TightenRope() {
        length = Vector2.Distance(Position, swingpoint);
    }

    /// <summary>
    /// For player input. Re-creates the rope starting from a point.
    /// </summary>
    /// <param name="point">End point of the rope.</param>
    public void Connect(Vector2 point) {
        if(connected) Disconnect();
        PointsAdd(point, null);
        swingpoint = point;
        TightenRope();
    }

    public void Disconnect() {
        if(Geometry.Exists(rigidbody.velocity) && connected)
            TangentVelocity(gameObject, swingpoint);
        PointsClear();
        distance = 0;
    }
    
    internal void PointsAdd(Vector2 point, Collider2D collider) {
        points.Add(point);
        colliders.Add(collider);
        Count++;
        foundEdge = false;
    }

    internal void PointsRemove() {
        points.RemoveAt(Count - 1);
        colliders.RemoveAt(Count - 1);
        Count--;
        foundEdge = false;
    }

    internal void PointsClear() {
        points.Clear();
        colliders.Clear();
        Count = 0;
        foundEdge = false;
    }
    #endregion

    #endregion
}
