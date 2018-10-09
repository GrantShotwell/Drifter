using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightRangeRandomizer : MonoBehaviour {
    public float multiplier = 0.1f;
    public float variance = 2;
    float baseRange;
    new Light light;
    
	void Start () {
        light = GetComponent<Light>();
        baseRange = light.range;
	}
    
	void Update () {
        light.range += Random.Range(-1f, 1f) * multiplier;
        float min = baseRange - variance, max = baseRange + variance;
        if(light.range < min) light.range = min + (Random.Range(0f, 1f) * multiplier);
        if(light.range > max) light.range = max - (Random.Range(0f, 1f) * multiplier);
	}
}
