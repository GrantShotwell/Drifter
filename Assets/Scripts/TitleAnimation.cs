using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour {
    float[] startX, startY;
    float waveHeightMultiplier = 10;
    float waveSpeedMultiplier = 3;
    float minY = 0;
    float minX = 0;

	void Start() {
        //relavtive x, y coords of the letters are saved so they can be moved around their original position in Update()
        startX = new float[transform.childCount];
        int j = 0;
        foreach(Transform child in transform) {
            startX[j] = child.transform.position.x;
            j++;
        }
        startY = new float[transform.childCount];
        int k = 0;
        foreach(Transform child in transform) {
            startY[k] = child.transform.position.y;
            k++;
        }
	}
	
	void Update() {
        int j = 0;
		foreach(Transform child in transform) {
            float x = Mathf.Sin(Time.time * waveSpeedMultiplier + j) * waveHeightMultiplier + startX[j];
            float y = Mathf.Sin(Time.time * waveSpeedMultiplier + j) * waveHeightMultiplier + startY[j];
            if(x < minX + startX[j]) x = startX[j];
            if(y < minY + startY[j]) y = startY[j];
            child.transform.position = new Vector3(x, y, 0);
            j++;
        }
	}
}
