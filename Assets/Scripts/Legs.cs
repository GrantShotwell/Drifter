using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour {
    /// <summary>
    /// Finds the segments of the leg by name
    /// Ie. "Any Name"(base)/"Segment"/"Segment"/.../"Foot Vector"(defines the end of the leg)
    /// with each following GameObject being the child of the previous
    /// </summary>

    public float height;
    public GameObject[] leftLegs;
    public GameObject[] rightLegs;
    public GameObject[] upperLegs;
    GameObject[][] legSets = new GameObject[3][];
    Vector2[][] footVectors = new Vector2[3][];
    float[][] legLengths = new float[3][];
    float[][][] segmentLengths = new float[3][][];
    GameObject[][][] segments = new GameObject[3][][];

    void Start() {
        legSets[0] = leftLegs;
        legSets[1] = rightLegs;
        legSets[2] = upperLegs;

        for(int j = 0; j < legSets.Length; j++) {
            int legSetSize = legSets[j].Length;
            footVectors[j] = new Vector2[legSetSize];
            legLengths[j] = new float[legSetSize];
            segments[j] = new GameObject[legSetSize][];
            segmentLengths[j] = new float[legSetSize][];

            for(int k = 0; k < legSets[j].Length; k++) {
                segments[j][k] = getSegmentList(transform.GetChild(k).gameObject).ToArray();
                segmentLengths[j][k] = new float[segments[j][k].Length];
                
                for(int l = 0; l < segments[j][k].Length; l++) {
                    foreach(Transform childTransform in segments[j][k][l].transform)
                        if(childTransform.gameObject.name.Equals("Foot Vector"))
                            footVectors[j][k] = childTransform.position;
                    segmentLengths[j][k][l] = SegmentLength(j, k, l);
                }

                legLengths[j][k] = LegLength(j, k);
            }
        }
    }

    void FixedUpdate() {
        Debug.Log(" - - - ");
        for(int j = 0; j < legSets.Length; j++) {
            GameObject[] legSet = legSets[j];

            for(int k = 0; k < legSet.Length; k++) {
                GameObject leg = legSet[k];
                float legLength = legLengths[j][k];
                Vector2 desiredEndpoint;

                float rotateAmount = 10.0f;
                Vector2 direction = new Vector2(0, 0);
                Vector2 desired = new Vector2(0, 0);
                switch(j) {
                    case 0:
                        direction = Vector2.left;
                        desired = Vector2.down;
                        break;
                    case 1:
                        direction = Vector2.right;
                        desired = Vector2.down;
                        break;
                    case 2:
                        direction = Vector2.up;
                        desired = Vector2.down;
                        break;
                }
                RaycastHit2D hit = Physics2D.Raycast(leg.transform.position, direction, legLength, ~(1<<8));
                float rotatedTotal = 0.0f;
                while(rotatedTotal < 90.0f && hit.collider == null) {
                    direction = direction.RotateTo(desired, rotateAmount);
                    rotatedTotal += rotateAmount;
                    hit = Physics2D.Raycast(leg.transform.position, direction, legLength, ~(1 << 8));
                }
                Debug.DrawRay(leg.transform.position, direction, Color.cyan, 1);

                if(hit.collider == null) desiredEndpoint = Vector2.down * (legLength) + (Vector2)leg.transform.position;
                else desiredEndpoint = hit.point;
                float angle = Vector2.Angle(desiredEndpoint - (Vector2)leg.transform.position, Vector2.right);
                for(int l = 0; l < segments[j][k].Length; l++) {
                    GameObject segment = segments[j][k][l];
                    float segmentLength = segmentLengths[j][k][l];
                    HingeJoint2D hinge = segment.GetComponent<HingeJoint2D>();

                    SetHingeAngle(hinge, angle * (segmentLength / legLength) * Geometry.direction(direction.x));
                    Debug.Log(angle);
                }
            }
        }
    }

    void SetHingeAngle(HingeJoint2D hinge, float angle) {
        hinge.useLimits = true;
        JointAngleLimits2D limit = hinge.limits;
        limit.min = angle;
        limit.max = angle;
        hinge.limits = limit;
    }

    List<GameObject> getSegmentList(GameObject parent) {
        GameObject endSegment = parent;
        bool foundEnd = false;

        List<GameObject> segmentList = new List<GameObject>();
        while(!foundEnd) {
            GameObject oldEndSegment = endSegment;
            foreach(Transform childTransform in endSegment.transform) {
                GameObject childObj = childTransform.gameObject;
                if(childObj.name.Equals("Segment")) {
                    segmentList.Add(childObj);
                    endSegment = childObj;
                }
            }
            if(endSegment == oldEndSegment) foundEnd = true;
        }

        return segmentList;
    }

    float LegLength(int J, int K) {
        float totalDistance = Vector2.Distance(
            legSets[J][K].transform.position,
            segments[J][K][0].GetComponent<HingeJoint2D>().anchor + (Vector2)segments[J][K][0].transform.position
        );
        for(int l = 1; l < segments[J][K].Length; l++) {
            GameObject prevSegment = segments[J][K][l - 1];
            GameObject thisSegment = segments[J][K][l];
            totalDistance += Vector2.Distance(
                prevSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)prevSegment.transform.position,
                thisSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)thisSegment.transform.position
            );
        }
        GameObject lastSegment = segments[J][K][segments[J][K].Length - 1];
        totalDistance += Vector2.Distance(
            lastSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)lastSegment.transform.position,
            footVectors[J][K]
        );
        Debug.Log(totalDistance);
        return totalDistance;
    }
    
    float SegmentLength(int J, int K, int L) {
        float length = 0;
        GameObject thisSegment = segments[J][K][L];
        if(L == segments[J][K].Length - 1) {
            length = Vector2.Distance(
                thisSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)thisSegment.transform.position,
                footVectors[J][K]
            );
        }
        else {
            GameObject nextSegment = segments[J][K][L + 1];
            length = Vector2.Distance(
                    thisSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)thisSegment.transform.position,
                    nextSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)nextSegment.transform.position
            );
        }
        return length;
    }
}
