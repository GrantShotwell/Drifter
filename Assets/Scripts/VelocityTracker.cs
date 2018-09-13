using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTracker : MonoBehaviour {
    public int storageSize = 100;

    Rigidbody2D rb;
    float[] previousSpeeds;
    int currentIndex = 0;
    float sum = 0;

    public float averageSpeed;

    void Start () {
        previousSpeeds = new float[storageSize];
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        float currentSpeed = Mathf.Sqrt(Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.y, 2));
        addNewSpeed(currentSpeed);
        updateAverage();
    }
    
    void addNewSpeed(float newSpeed) {
        sum += newSpeed;
        sum -= previousSpeeds[currentIndex];
        previousSpeeds[currentIndex] = newSpeed;
        currentIndex++;
        if(currentIndex >= previousSpeeds.Length) currentIndex = 0;
    }

    void updateAverage() {
        averageSpeed = sum / previousSpeeds.Length;
    }
}
