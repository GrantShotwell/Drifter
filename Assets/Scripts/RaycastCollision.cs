using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RaycastCollision : MonoBehaviour {
    public float size, offset;
    bool inCollision = false;
    new Rigidbody2D rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if(!inCollision) {
            RaycastHit2D hit = Physics2D.CircleCast(rigidbody.position, size, rigidbody.velocity, rigidbody.velocity.magnitude * Time.fixedDeltaTime);
            if(hit.collider != null && hit.distance > offset) {
                Vector2 hitVector = hit.point - rigidbody.position;
                hitVector = hitVector.SetMagnitude(hitVector.magnitude - offset);
                Debug.DrawLine(rigidbody.position, rigidbody.position + hitVector);
                rigidbody.position += hitVector;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) { inCollision = true; }
    private void OnCollisionStay2D(Collision2D collision)  { inCollision = true; }
    private void OnCollisionExit2D(Collision2D collision) { inCollision = false; }
}
