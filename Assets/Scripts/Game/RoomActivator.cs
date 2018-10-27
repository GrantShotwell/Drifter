using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomActivator : MonoBehaviour {
    public GameObject room;
    List<GameObject> persistants = new List<GameObject>();

    private void Update() { room.SetActive(persistants.Count > 0); }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.tag == "Persistant")
            persistants.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if(collision.gameObject.tag == "Persistant")
            persistants.Remove(collision.gameObject);
    }
}
