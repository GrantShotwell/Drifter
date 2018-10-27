using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throw : MonoBehaviour {
    [MinMaxRange(-5.0f, 5.0f)]
    public RangedFloat X;
    public float Y;

    public void ThrowCollider(Collider2D collider) { ThrowObject(collider.gameObject); }
    public void ThrowObject(GameObject gameObject) {
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(X.Min, X.Max), Y) * gameObject.GetComponent<Rigidbody2D>().mass, ForceMode2D.Impulse);
    }
}
