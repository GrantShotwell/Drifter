using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIgnorer : MonoBehaviour {
    public GameObject[] ignoreList;
    BoxCollider2D box;

    //the ignore needs to be faster than the physics update, but the physics update comes before the ignore list can be set, so collisions are immediately disabled.
    private void OnEnable() {
        box = GetComponent<BoxCollider2D>();
        box.enabled = false;
    }

    private void Start() {
        foreach(GameObject obj in ignoreList)
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), obj.GetComponent<Collider2D>());
        box.enabled = true;
    }

    public void Ignore(GameObject objectToIgnore) {
        ignoreList = new GameObject[1];
        ignoreList[0] = objectToIgnore;
    }

    public void Ignore(GameObject[] objectsToIgnore) {
        ignoreList = objectsToIgnore;
    }
}
