using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Rope : MonoBehaviour {
    #region Variables
    [Header("Attributes")]

    public Color color = Color.white;

    public GameObject ropeCollider;

    [Header("Values")]

    public float retractRate = 1.0f;

    public Vector2 endpoint;
    public bool connected { get; private set; } = false;
    public float length { get; private set; } = 0;
    private float distance = 0;
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
    
    private void FixedUpdate() {
        if(connected) {
            distance = Vector2.Distance(transform.position, endpoint);
            if(distance > length) {
                //lerp position -> endpoint// keep gameObject within length of the rope
                float posLerpAmount = (distance - length) / distance;
                transform.position = Vector2.Lerp(transform.position, endpoint, posLerpAmount);

                if(Geometry.Exists(rigidbody.velocity)) {
                    rigidbody.velocity = TangentVelocity(gameObject, endpoint);

                    //'give back' the energy it lost from moving it's position
                    if(length != 0) {
                        float ratio = (distance - length) / length;
                        rigidbody.velocity *= 1 + ratio;
                    }
                }

                distance = length;
            }
        }
    }
    #endregion

    #region Functions
    public void ShortenRope(float amount) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, endpoint - (Vector2)transform.position);
        if(hit.distance > amount)
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
    #endregion
}
