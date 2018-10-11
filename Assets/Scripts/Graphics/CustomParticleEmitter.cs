using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomParticleEmitter : MonoBehaviour {
    public GameObject particle;
    public int count;
    public bool selfDestruct;
    [ConditionalField("selfDestruct")] public float time;
    [ConditionalField("selfDestruct", false)] public bool force;

    [System.Serializable]
    public class Range {
        public float minimum;
        public float maximum;
    }

    public bool giveSpeed;
    [ConditionalField("giveSpeed")] public Range speed = new Range();

    public bool givePosition;
    [ConditionalField("givePosition")] public Range position = new Range();

    private void Start() {
        for(int j = 0; j < count; j++) {
            GameObject p = Instantiate(particle, gameObject.transform);

            if(givePosition) {
                Transform transform = p.GetComponent<Transform>();
                if(transform != null) {
                    Vector2 pos = Vector2.right * Random.Range(position.minimum, position.maximum);
                    pos = pos.Rotate(Random.Range(0, 360));
                    transform.position += (Vector3)pos;
                }
            }

            if(giveSpeed) {
                Rigidbody2D rigidbody = p.GetComponent<Rigidbody2D>();
                if(rigidbody != null) {
                    Vector2 velocity = Vector2.right * Random.Range(speed.minimum, speed.maximum);
                    velocity = velocity.Rotate(Random.Range(0, 360));
                    rigidbody.velocity += velocity;
                }
            }

            if(selfDestruct) {
                SelfDestruct destructor = p.GetComponent<SelfDestruct>();
                if(destructor != null)
                    destructor.timeToExist = time;
                else {
                    destructor = p.AddComponent<SelfDestruct>();
                    destructor.timeToExist = time;
                }
            }
            else if(force) {
                SelfDestruct destructor = p.GetComponent<SelfDestruct>();
                if(destructor != null) {
                    Destroy(destructor);
                }
            }
        }
    }
}
