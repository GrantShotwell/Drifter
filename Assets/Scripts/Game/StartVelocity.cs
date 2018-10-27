using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVelocity : MonoBehaviour {
    public Vector2 velocity;
    private void Start() { GetComponent<Rigidbody2D>().velocity = velocity; }
}
