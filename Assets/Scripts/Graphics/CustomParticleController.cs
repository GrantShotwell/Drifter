using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParticleController : MonoBehaviour {
    public GameObject emitter;

    public void Emit(Vector2 position) {
        Instantiate(emitter).transform.position = position;
    }
}
