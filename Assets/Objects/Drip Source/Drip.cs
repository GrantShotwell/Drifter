using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class Drip : MonoBehaviour {
    [HideInInspector]
    public DripSource source;
    bool freefall = false;

    private void OnCollisionEnter2D(Collision2D collision) {
        if(freefall) source.SendMessage("DripHitGround", gameObject);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(!freefall) source.SendMessage("DripFreefall", gameObject);
        freefall = true;
    }
}

public interface IDripSource : IEventSystemHandler {
    void DripFreefall(GameObject gameObject);
    void DripHitGround(GameObject gameObject);
}
