using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIgnorer : MonoBehaviour {
    public GameObject ignored;
    Collider2D[] colliders;

    //the ignore needs to be faster than the physics update, but the physics update comes before the ignore list can be set, so collisions are immediately disabled.
    private void OnEnable() {
        colliders = GetComponents<Collider2D>();
        foreach(Collider2D collider in colliders) {
            collider.enabled = false;
        }
    }

    private void Start() {
        foreach(Collider2D colliderThis in colliders) {
            foreach(Collider2D colliderOthr in ignored.GetComponents<Collider2D>()) {
                Physics2D.IgnoreCollision(colliderThis, colliderOthr);
            }
            colliderThis.enabled = true;
        }
    }

    public void Ignore(GameObject objectToIgnore) { ignored = objectToIgnore; }
}
