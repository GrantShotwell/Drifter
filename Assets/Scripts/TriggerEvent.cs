using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {
    [Header("Behavior")]
    public bool ifList = false;
    public GameObject[] gameObjects;

    [Header("Events")]
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision) {
        bool condition = false;
        if(ifList) {
            foreach(GameObject gameObject in gameObjects)
                if(gameObject == collision.gameObject) condition = true;
        }
        else condition = true;
        if(condition) onTriggerEnter.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        bool condition = false;
        if(ifList) {
            foreach(GameObject gameObject in gameObjects)
                if(gameObject == collision.gameObject) condition = true;
        }
        else condition = true;
        if(condition) onTriggerStay.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        bool condition = false;
        if(ifList) {
            foreach(GameObject gameObject in gameObjects)
                if(gameObject == collision.gameObject) condition = true;
        }
        else condition = true;
        if(condition) onTriggerExit.Invoke();
    }
}
