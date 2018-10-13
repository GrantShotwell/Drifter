using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hover : MonoBehaviour {
    [Header("Height Authority")]

    [Tooltip("Target height above the ground.")]
    public float height;

    [Tooltip("Distance above the target height where the script will apply a downwards force. This is to counteract issues with bouncing.")]
    public float limit;

    [Tooltip("The script determines height above the ground by raycasting. If the base of this object is large, consider adding more raycasts.")]
    public Vector2[] raycastStartVectors;

    [Tooltip("The direction of all of the raycasts above. This should be the same direction as gravity.")]
    public Vector2 direction;

    [Header("Forces")]

    [Tooltip("This is the maximum force that the script will apply, opposite of the direction vector. Magnitude gets closer to maximum as the object gets closer to the ground.")]
    public float force;

    [Tooltip("Multiply the magnitude of the force by the mass of this object (from RigidBody2D).")]
    public bool multiplyByMass;

    private bool _onGround = false;
    private bool _nearGround = false;
    new Rigidbody2D rigidbody;

    public bool onGround { get { return _onGround; } }
    public bool nearGround { get { return _nearGround; } }

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        if(raycastStartVectors.Length == 0) Debug.LogWarning("There needs to be at least one raycast vector in the list for the script to do anything.");
        if(direction.x == 0 && direction.y == 0) Debug.LogWarning("Raycasting will not work if its direction vector is zero.");
        if(rigidbody.drag == 0) Debug.LogWarning("It is recommended that Rigidbody2D.drag is turned on to avoid agressive bouncing.");
    }

    private void FixedUpdate() {
        direction.Normalize();
        float lowestDistance = 0;
        _onGround = false;
        _nearGround = false;
        for(int j = 0; j < raycastStartVectors.Length; j++) {
            Vector2 startVector = raycastStartVectors[j] + (Vector2)transform.position;
            RaycastHit2D hit = Physics2D.Raycast(startVector, direction, Mathf.Infinity);
            if(hit.distance < lowestDistance || j == 0) lowestDistance = hit.distance;
        }
        if(lowestDistance <= height) _onGround = true;
        else if(lowestDistance <= height + limit) _nearGround = true;
        if(_onGround || _nearGround) {
            Vector2 F = direction * force;
            if(multiplyByMass) F *= rigidbody.mass;

            if(_onGround) F *= (height - lowestDistance) / height;
            if(_nearGround) F *= (lowestDistance - height) / limit;

            if(_onGround) rigidbody.AddForce(-F); //lift up
            if(_nearGround) rigidbody.AddForce(F); //push down
        }
    }
}
