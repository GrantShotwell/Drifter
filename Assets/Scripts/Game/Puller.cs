using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puller : MonoBehaviour {
    [Header("Attributes")]

    [Tooltip("Color of the line.")]
    public Color color;

    [Header("Values")]

    [Tooltip("Rate in degrees per second that the velocity is pointed toward the endpoint. Also determines how small the angle has to be to start pulling.")]
    public float rotateRate = 1.0f;

    [Tooltip("Rate in units per second that the GameObject is pulled in once the angle is correct.")]
    public float retractRate = 1.0f;

    [Tooltip("Max speed that the puller will bring the GameObject to. 'Infinity' means there is no limit. '0' means it will only pull when stationary.")]
    public float maxSpeedToPull = 1.0f;

    [Tooltip("Distance at which the puller breaks. 'Infinity' means that it will never break due to distane. '0' means it will break instantly.")]
    public float maxDistance = Mathf.Infinity;

    [Tooltip("Time in seconds between when the puller was fired, and when it can be fired again. 'Infinity' means it can never be fired again. '0' means there is no cooldown.")]
    public float cooldownTime = 3.0f;

    [Tooltip("Time in seconds that the puller has until it breaks starting from when Connect() is called. 'Infinity' means it will never break due to time. '0' means it will break instantly.")]
    public float activeTime = 1.0f;

    private float lastTimeActive = -999999;

    [Tooltip("//todo//")]
    public bool effects = true;

    Vector2 endpoint = new Vector2(0, 0);
    bool connected = false;
    public bool isConnected { get { return connected; } }
    float distance;
    new Rigidbody2D rigidbody;
    LineRenderer lineRenderer;

    void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Update() {
        if(connected) {
            Vector3[] points = { transform.position, endpoint };
            lineRenderer.SetPositions(points);
        }
    }

    private void FixedUpdate() {
        distance = Vector2.Distance(transform.position, endpoint);
        if(Time.time - lastTimeActive - activeTime > 0) Disconnect();
        if(connected) {
            if(distance > maxDistance)
                Disconnect();
            else if(distance > retractRate) {
                Vector2 relativeEndpoint = endpoint - (Vector2)transform.position;
                Vector2 adjustedVelocity = rigidbody.velocity * Time.fixedDeltaTime;

                float angle = Vector2.Angle(adjustedVelocity, relativeEndpoint);
                if(rigidbody.velocity.magnitude < maxSpeedToPull || angle < rotateRate) {
                    LerpVelocity(relativeEndpoint, adjustedVelocity);
                }
                else if(angle >= rotateRate) {
                    adjustedVelocity = adjustedVelocity.RotateTo(relativeEndpoint, rotateRate);
                    adjustedVelocity /= rotateRate / 360;
                    rigidbody.velocity = adjustedVelocity;
                }
            }
        }
    }

    void LerpVelocity() {
        Vector2 relativeEndpoint = endpoint - (Vector2)transform.position;
        Vector2 adjustedVelocity = rigidbody.velocity * Time.fixedDeltaTime;
        LerpVelocity(relativeEndpoint, adjustedVelocity);
    }
    void LerpVelocity(Vector2 relativeEndpoint, Vector2 adjustedVelocity) {
        adjustedVelocity = Vector2.LerpUnclamped(adjustedVelocity, relativeEndpoint, retractRate * Time.fixedDeltaTime / distance);
        rigidbody.velocity = adjustedVelocity / Time.fixedDeltaTime;
    }

    public void Connect(Vector2 point) { Connect(point, false); }
    public void Connect(Vector2 point, bool ignoreCooldown) {
        if(ignoreCooldown || Time.time - lastTimeActive - cooldownTime >= 0) {
            Rope rope = GetComponent<Rope>();
            if(rope != null) rope.Disconnect();

            lastTimeActive = Time.time;
            endpoint = point;
            connected = true;

            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    public void Disconnect() {
        connected = false;

        Vector3[] points = { new Vector2(0, 0), new Vector2(0, 0) };
        lineRenderer.SetPositions(points);
    }
}
