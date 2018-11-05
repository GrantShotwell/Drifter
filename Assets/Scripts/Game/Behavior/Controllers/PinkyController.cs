using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinkyController : MonoBehaviour {
    public float lifetime = 20;
    public int maxBounces;
    public int bounces { get; private set; } = 0;
    public bool destroyOnDamage;
    public float startTime { get; private set; }
    new Rigidbody2D rigidbody;

    private void Start() {
        startTime = Time.time;
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if(Time.time > startTime + lifetime) Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        rigidbody.velocity = Vector2.Reflect(rigidbody.velocity, collision.GetContact(0).normal);
        bounces++;
        if(bounces >= maxBounces) Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision) {
        Vector2 normal = collision.GetContact(0).normal;
        float magnitude = collision.GetContact(0).normalImpulse;
        if(Vector2.Dot(rigidbody.velocity, -normal) > 0) rigidbody.position += normal * magnitude;
    }

    private void OnDamage() {
        if(destroyOnDamage) Destroy(gameObject);
    }
}
