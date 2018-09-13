using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRope : MonoBehaviour {
    public Color ropeColor = Color.white;
    public float ropeShortenLength = 1f;
    Vector2 endpoint = new Vector2(0, 0);
    Vector2 mousePos;
    bool connected = false;
    float length = 0;
    float distance = 0;
    new Rigidbody2D rigidbody;
    LineRenderer lineRenderer;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update() {
        if(connected) {
            Vector3[] points = { transform.position, endpoint };
            lineRenderer.SetPositions(points);
        }
        if(Input.GetButtonDown("Destroy Rope")) {
            Disconnect();
        }
    }

    void LateUpdate() {
        //mouse update is required on LateUpdate because camera is still moving on Update
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(Input.GetButtonDown("Make Rope")) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, Mathf.Infinity, ~(1<<8));
            if(hit.collider != null) Connect(hit.point);
        }
    }

    float lastMagnitude;
    Vector2 lastPosition;
    float velocityMoved;
    void FixedUpdate() {
        velocityMoved = Vector2.Distance(lastPosition, transform.position) / lastMagnitude;
        
        if(Input.GetButton("Shorten Rope")) {
            length -= ropeShortenLength;
            if(length <= 0) length = ropeShortenLength;
        }

        distance = Vector2.Distance(transform.position, endpoint);
        if(connected && distance > length) {
            //lerp position -> endpoint// keep gameObject within length of the rope
            float posLerpAmount = (distance - length) / distance;
            transform.position = Vector2.Lerp(transform.position, endpoint, posLerpAmount);

            //'lerp' velocity -> endpoint// keep the velocity locked to the tangent of the circle around the endpoint
            Vector2 adjustedVelocity = rigidbody.velocity * velocityMoved;
            Vector2 relativeVelocity = adjustedVelocity + (Vector2)transform.position;
            Line l1 = Geometry.LineFromTwoPoints(relativeVelocity, endpoint);
            Line l2 = Geometry.LineFromAngle(transform.position, Geometry.GetAngle(endpoint, transform.position) - 90);
            if(!Geometry.AreParallel(l1, l2)) {
                Vector2 pointToLerpTo = Geometry.Intersection(l1, l2) - (Vector2)transform.position;
                if(!(float.IsNaN(pointToLerpTo.x) || float.IsNaN(pointToLerpTo.y) || float.IsInfinity(pointToLerpTo.x) || float.IsInfinity(pointToLerpTo.y))) {
                    rigidbody.velocity = pointToLerpTo;
                    rigidbody.velocity /= velocityMoved;
                }
            }
            else rigidbody.velocity = new Vector2(0, 0);

            //'give back' the energy it lost from moving it's position
            float ratio = (distance - length) / length;
            rigidbody.velocity *= 1 + ratio;

            distance = length;
        }
        lastPosition = transform.position;
        lastMagnitude = rigidbody.velocity.magnitude;
    }

    void TightenRope() {
        length = Vector2.Distance(transform.position, endpoint);
    }

    void Connect(Vector2 point) { Connect(point, true); }
    void Connect(Vector2 point, bool tighten) {
        endpoint = point;
        if(tighten) TightenRope();
        connected = true;
    }

    void Disconnect() {
        connected = false;
        Vector3[] points = { new Vector2(0, 0), new Vector2(0, 0) };
        lineRenderer.SetPositions(points);

        //'lerp' the velocity one last time to make sure the disconnect is smooth
        if(distance >= length) {
            Vector2 adjustedVelocity = rigidbody.velocity * velocityMoved;
            Vector2 relativeVelocity = adjustedVelocity + (Vector2)transform.position;
            Line l1 = Geometry.LineFromTwoPoints(relativeVelocity, endpoint);
            Line l2 = Geometry.LineFromAngle(transform.position, Geometry.GetAngle(endpoint, transform.position) - 90);
            if(!Geometry.AreParallel(l1, l2)) {
                Vector2 pointToLerpTo = Geometry.Intersection(l1, l2);
                rigidbody.velocity = pointToLerpTo - (Vector2)transform.position;
                rigidbody.velocity /= velocityMoved;
            }
        }
    }
}
