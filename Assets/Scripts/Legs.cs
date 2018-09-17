using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour {
    /// <summary>
    /// Finds the segments of the leg by name
    /// Ie. "Any Name"(base)/"Segment"/"Segment"/.../"Foot Vector"(defines the end of the leg)
    /// with each following GameObject being the child of the previous
    /// </summary>
    
    public GameObject[] leftLegs;
    public GameObject[] rightLegs;
    public GameObject[] upperLegs;
    GameObject[][] legSets = new GameObject[3][];
    Vector2[][] footVectors = new Vector2[3][];
    float[][] legLengths = new float[3][];
    GameObject[][][] segments = new GameObject[3][][];

    void Start() {
        legSets[0] = leftLegs;
        legSets[1] = rightLegs;
        legSets[2] = upperLegs;

        footVectors[0] = new Vector2[leftLegs.Length];
        footVectors[1] = new Vector2[rightLegs.Length];
        footVectors[2] = new Vector2[upperLegs.Length];

        legLengths[0] = new float[leftLegs.Length];
        legLengths[1] = new float[rightLegs.Length];
        legLengths[2] = new float[upperLegs.Length];

        for(int j = 0; j < legSets.Length; j++) {
            segments[j] = new GameObject[legSets[j].Length][];
            for(int k = 0; k < legSets[j].Length; k++) {
                segments[j][k] = getSegmentList(transform.GetChild(k).gameObject).ToArray();
                for(int l = 0; l < segments[j][k].Length; l++)
                    foreach(Transform childTransform in segments[j][k][l].transform)
                        if(childTransform.gameObject.name.Equals("Foot Vector"))
                            footVectors[j][k] = childTransform.position;
                legLengths[j][k] = LegLength(j, k);
            }
        }
    }

    List<GameObject> getSegmentList(GameObject parent) {
        GameObject endSegment = parent;
        foreach(Transform childTransform in parent.transform) {
            GameObject childObj = childTransform.gameObject;
            if(childObj.name.Equals("Segment")) endSegment = childObj;
        }

        bool foundEnd = false;
        if(endSegment == parent) foundEnd = true;

        //finds the bottom
        while(!foundEnd) {
            GameObject oldEndSegment = endSegment;
            foreach(Transform childTransform in endSegment.transform) {
                GameObject childObj = childTransform.gameObject;
                if(childObj.name.Equals("Segment")) endSegment = childObj;
            }
            if(endSegment == oldEndSegment) foundEnd = true;
        }

        //goes back up to the top (adding all of the segments to the list along the way)
        List<GameObject> segmentList = new List<GameObject>();
        GameObject current = endSegment;
        while(current != parent) {
            segmentList.Insert(0, current);
            current = current.transform.parent.gameObject;
        }
        return segmentList;
    }

    float LegLength(int J, int K) {
        float totalDistance = Vector2.Distance(legSets[J][K].transform.position, segments[J][K][0].transform.position);
        Debug.Log(totalDistance + " <-- " + legSets[J][K].transform.position + ", " + segments[J][K][0].transform.position);
        for(int l = 1; l < segments[J][K].Length; l++) {
            totalDistance += Vector2.Distance(segments[J][K][l-1].transform.position, segments[J][K][l].transform.position);
            Debug.Log(totalDistance + " <-- " + segments[J][K][l - 1].transform.position + ", " + segments[J][K][l].transform.position);
        }
        totalDistance += Vector2.Distance(segments[J][K][segments[J][K].Length-1].transform.position, footVectors[J][K]);
        Debug.Log(totalDistance + " <-- " + segments[J][K][segments[J][K].Length - 1].transform.position + ", " + footVectors[J][K]);
        return totalDistance;
    }
    
    void FixedUpdate() {

    }
}
