using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour {
    public GameObject target;
    public float force;
    public float maxSpeed;
    public bool ignoreX;
    public bool ignoreY;
    public float minimumDistance;
    public float maximumDistance;
    public bool keepDistance;
    new Rigidbody2D rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        Debug.Assert(target != null, "'Follow.target' must exist. (add a target via the inspector)");
    }

    private void FixedUpdate() {
        float distance = Vector2.Distance(transform.position, target.transform.position);
        float speed = rigidbody.velocity.magnitude;
        if(maximumDistance >= distance && distance > minimumDistance && speed < maxSpeed) {
            Vector2 F = new Vector2(0, 0);
            if(ignoreX != ignoreY) {
                if(!ignoreX) F = new Vector2(force * Geometry.Direction(target.transform.position.x - transform.position.x), 0);
                if(!ignoreY) F = new Vector2(0, force * Geometry.Direction(target.transform.position.y - transform.position.y));
            }
            else if(!(ignoreX && ignoreY)) {
                F = (Vector2.right * force).Rotate(Geometry.GetAngle(transform.position, target.transform.position));
            }
            rigidbody.AddForce(F);
        }

        if(distance < minimumDistance && keepDistance) {
            Vector2 F = new Vector2(0, 0);
            if(ignoreX != ignoreY) {
                if(!ignoreX) F = new Vector2(force * Geometry.Direction(target.transform.position.x - transform.position.x), 0);
                if(!ignoreY) F = new Vector2(0, force * Geometry.Direction(target.transform.position.y - transform.position.y));
            }
            else if(!(ignoreX && ignoreY)) {
                F = (Vector2.right * force).Rotate(Geometry.GetAngle(transform.position, target.transform.position));
            }
            rigidbody.AddForce(-F);
        }
    }
}
