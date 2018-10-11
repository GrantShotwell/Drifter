using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveStartMomentum : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity += new Vector2(0, 50);
	}
}
