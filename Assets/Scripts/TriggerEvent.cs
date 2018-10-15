using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerEvent : MonoBehaviour {
    public UnityEvent onTriggerEnter;
    public UnityEvent onTriggerStay;
    public UnityEvent onTriggerExit;

    private void OnTriggerEnter2D(Collider2D collision) {
        onTriggerEnter.Invoke();
    }

    private void OnTriggerStay2D(Collider2D collision) {
        onTriggerStay.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        onTriggerExit.Invoke();
    }
}
