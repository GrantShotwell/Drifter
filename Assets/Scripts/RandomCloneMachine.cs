using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomCloneMachine : MonoBehaviour {
    public int amount = 100;
    public float xMax = 1000;
    public float xMin = -1000;
    public float yMax = 1000;
    public float yMin = -1000;
    public float sMin = 1;
    public float sMax = 10;

    bool isClone = false;
    GameObject clone;

    void Start() {
        if(isClone == false) {
            for(int j = 0; j <= amount; j++) {
                clone = Instantiate(gameObject);
                clone.transform.position = new Vector3(Random.Range(xMin, xMax), Random.Range(yMin, yMax), 0);
                clone.transform.localScale = new Vector3(Random.Range(sMin, sMax), Random.Range(sMin, sMax), 1);
                clone.GetComponent<RandomCloneMachine>().isClone = true;
            }
        }
    }
}
