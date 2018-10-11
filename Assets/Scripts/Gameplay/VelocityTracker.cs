using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTracker : MonoBehaviour {
    [Tooltip("Number of velocities to calculate the average. The larger the value, the less the average will be affected by outliers.")]
    public int storageSize = 100;
    
    float[] previousSpeeds;
    int currentIndex = 0;
    float sum = 0;
    public float averageSpeed { get; private set; }

    void Start () {
        previousSpeeds = new float[storageSize];
    }

    void FixedUpdate() {
        float currentSpeed = GetComponent<Rigidbody2D>().velocity.magnitude;
        addNewSpeed(currentSpeed);
        updateAverage();
    }
    
    void addNewSpeed(float newSpeed) {
        sum -= previousSpeeds[currentIndex];
        previousSpeeds[currentIndex] = newSpeed;
        sum += newSpeed;

        currentIndex++;
        if(currentIndex >= previousSpeeds.Length) currentIndex = 0;
    }

    void updateAverage() {
        averageSpeed = sum / previousSpeeds.Length;
    }
}
