using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rope : MonoBehaviour {
    #region Variables
    [Header("Attributes")]

    public Color color = Color.white;

    [Header("Values")]

    public float retractRate = 1.0f;

    public Vector2 endpoint { get; private set; }
    public bool connected { get; private set; } = false;
    public float length { get; private set; } = 0;
    public float distance { get; private set; } = 0;
    new Rigidbody2D rigidbody;
    LineRenderer lineRenderer;
    #endregion

    #region Update
    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if(connected) {
            Vector3[] points = { endpoint, transform.position };
            lineRenderer.SetPositions(points);
        }
    }

    bool hasTension;
    private void FixedUpdate() {
        if(connected) {
            distance = Vector2.Distance(transform.position, endpoint);
            hasTension = (distance > length);

            if(hasTension) {
                #region G _perpendicular (Gp)
                //solving for the perpendicular component of gravity relative to the endpoint.

                float G = Physics2D.gravity.magnitude;
                float ThetaGE = Vector2.SignedAngle(
                    endpoint - (Vector2)transform.position,
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

                Vector2 p = Vector2.Lerp(transform.position, endpoint, (distance - length) / distance);
                Vector2 v = TangentVelocity(gameObject, endpoint);
                v *= ((distance - length) / length) + 1.0f;
                v += Gp * Time.fixedDeltaTime;

                rigidbody.MovePosition((v * Time.fixedDeltaTime) + p);
                rigidbody.velocity = v;

                distance = length;
            }
        }
        else hasTension = false;
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if(hasTension) length = distance;
    }
    #endregion

    #region Functions
    public void ShortenRope(float amount) {
        length -= amount;
    }

    public void TightenRope() {
        length = Vector2.Distance(transform.position, endpoint);
    } 
    
    public void Connect(Vector2 point) {
        Puller puller = GetComponent<Puller>();
        if(puller != null) puller.Disconnect();
        
        endpoint = point;
        TightenRope();
        connected = true;

        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }
    
    public void Disconnect() {
        Vector3[] points = { new Vector2(0, 0), new Vector2(0, 0) };
        lineRenderer.SetPositions(points);
        
        if(Geometry.Exists(rigidbody.velocity) && connected)
            TangentVelocity(gameObject, endpoint);
        
        connected = false;
        distance = 0;
    }
    
    public static Vector2 TangentVelocity(GameObject gameObj, Vector2 endpoint) {
        //return the lerped velocity so that it is tangent to the circle (the circle being one with a center at 'endpoint')

        Rigidbody2D rigidbody = gameObj.GetComponent<Rigidbody2D>();
        Transform transform = gameObj.transform;
        Vector2 newVelocity = rigidbody.velocity;

        Vector2 adjustedVelocity = rigidbody.velocity * Time.fixedDeltaTime;
        Vector2 relativeVelocity = adjustedVelocity + (Vector2)transform.position;
        Geometry.Line l1 = Geometry.LineFromTwoPoints(relativeVelocity, endpoint);
        Geometry.Line l2 = Geometry.LineFromAngle(transform.position, Geometry.GetAngle(endpoint, transform.position) - 90);
        if(!Geometry.AreParallel(l1, l2)) {
            Vector2 pointToLerpTo = Geometry.Intersection(l1, l2) - (Vector2)transform.position;
            if(Geometry.Exists(pointToLerpTo)) {
                newVelocity = pointToLerpTo;
                newVelocity /= Time.fixedDeltaTime;
            }
        }

        return newVelocity;
    }
    #endregion
}
