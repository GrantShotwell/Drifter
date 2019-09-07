using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;
using MyStuff.GeometryObjects;

public class SlimeGuyController : MonoBehaviour {

    public float sight,
        verticalForce,
        horizontalForce,
        delay,
        horizontalTime;

    GameObject player;
    TimedActionTracker timedActions = new TimedActionTracker();
    new Rigidbody2D rigidbody;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private bool tryJump = true;
    private void Update() {
        timedActions.Update();
        if(tryJump) {
            tryJump = false;
            if(Vector2.Distance(player.transform.position, transform.position) <= sight) {
                timedActions.AddAction(new TimedAction(delay, () => { Jump(); tryJump = true; }));
                timedActions.AddAction(new ContinuousAction(horizontalTime, () => {
                    rigidbody.AddForce(new Vector2(horizontalForce * Geometry.LinearDirection(player.transform.position.x - transform.position.x), 0f));
                }));
            } else {
                timedActions.AddAction(new TimedAction(Random.value, () => { tryJump = true; }));
            }
        }
    }

    public void Jump() {
        Vector2 force = new Vector2(horizontalForce * Geometry.LinearDirection(player.transform.position.x - transform.position.x), verticalForce);
        rigidbody.AddForce(force, ForceMode2D.Impulse);
    }

}
