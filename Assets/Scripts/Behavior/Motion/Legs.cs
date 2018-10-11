using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Legs : MonoBehaviour {
    /// <summary>
    /// Finds the segments of the leg by name
    /// Ie. "Any Name"(base)/"Segment"/"Segment"/.../"Foot Vector"(defines the end of the leg)
    /// with each following GameObject being the child of the previous
    /// </summary>

    public bool fixedUpdate = true;
    public float bendy = 0.8f;
    public float legResetSpeed = -1;
    public int legResetTime = 5;
    public GameObject[] leftLegs;
    public GameObject[] rightLegs;
    public GameObject[] upperLegs;

    GameObject[][] legSets = new GameObject[3][];
    GameObject[][][] segments = new GameObject[3][][];
    Vector2[][] footVectors = new Vector2[3][];
    Vector2[][] endpoints = new Vector2[3][];
    float[][] legLengths = new float[3][];
    float[][][] segmentLengths = new float[3][][];
    bool[][] tempEndpoint = new bool[3][];
    new Rigidbody2D rigidbody;

    public float rotateAmount = 10.0f;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();

        legSets[0] = leftLegs;
        legSets[1] = rightLegs;
        legSets[2] = upperLegs;

        for(int j = 0; j < legSets.Length; j++) {
            int legSetSize = legSets[j].Length;
            footVectors[j] = new Vector2[legSetSize];
            legLengths[j] = new float[legSetSize];
            segments[j] = new GameObject[legSetSize][];
            segmentLengths[j] = new float[legSetSize][];
            endpoints[j] = new Vector2[legSetSize];
            tempEndpoint[j] = new bool[legSetSize];

            for(int k = 0; k < legSets[j].Length; k++) {
                segments[j][k] = getSegmentList(transform.Find(legSets[j][k].name).gameObject);
                segmentLengths[j][k] = new float[segments[j][k].Length];
                tempEndpoint[j][k] = true;
                
                for(int l = 0; l < segments[j][k].Length; l++) {
                    foreach(Transform childTransform in segments[j][k][l].transform)
                        if(childTransform.gameObject.name.Equals("Foot Vector"))
                            footVectors[j][k] = childTransform.position;
                    segmentLengths[j][k][l] = SegmentLength(j, k, l);
                }

                legLengths[j][k] = LegLength(j, k);
                endpoints[j][k] = getEndpoint(j, k, true);
            }
        }
    }

    float timeSinceFast = 0;
    void Update() {
        if(!fixedUpdate) {
            UpdateLegs();
            if(rigidbody.velocity.magnitude <= legResetSpeed) timeSinceFast += Time.deltaTime;
            else timeSinceFast = 0;
            if(timeSinceFast >= legResetTime) ResetAllLegs();
        }
    }

    float updatesSinceFast = 0;
    private void FixedUpdate() {
        if(fixedUpdate) {
            UpdateLegs();
            if(rigidbody.velocity.magnitude <= legResetSpeed) updatesSinceFast++;
            else updatesSinceFast = 0;
            if(updatesSinceFast >= legResetTime) ResetAllLegs();
        }
    }

    void UpdateLegs() {
        for(int j = 0; j < legSets.Length; j++) {
            for(int k = 0; k < legSets[j].Length; k++) {
                Debug.Assert(segments[j][k] != null, "There are one or more legs that don't exist. Make sure the arrays aren't larger than it needs to be. (add legs via the inspector)");
                for(int l = 0; l < segments[j][k].Length; l++) {
                    foreach(Transform childTransform in segments[j][k][l].transform)
                        if(childTransform.gameObject.name.Equals("Foot Vector"))
                            footVectors[j][k] = childTransform.position;
                }
            }
        }

        for(int j = 0; j < legSets.Length; j++) {
            GameObject[] legSet = legSets[j];
            bool left = j % 2 == 0;
            Vector2 baseVector = Vector2.right;
            if(left) baseVector = Vector2.left;

            for(int k = 0; k < legSet.Length; k++) {
                GameObject leg = legSet[k];
                float legLength = legLengths[j][k];
                int segmentCount = segments[j][k].Length;
                Vector2[] segmentVectors = GetSegmentVectors(j, k);

                Vector2 endpoint = getEndpoint(j, k, false);
                //Debug.DrawLine(leg.transform.position, endpoint, Color.red);

                float endpointAngle = Vector2.SignedAngle(Vector2.right, endpoint - (Vector2)leg.transform.position);
                endpointAngle *= baseVector.x;
                float endpointDistance = Vector2.Distance(endpoint, leg.transform.position);

                float m = 1; if(left) m = -1;
                for(int x = 0; x < segmentVectors.Length; x++) segmentVectors[x] = new Vector2(m * segmentVectors[x].magnitude, 0);

                float angleSum = (segmentCount - 1) / 180;
                float remainingSum = legLength - segmentLengths[j][k][0];
                float a = segmentLengths[j][k][0], b = endpointDistance, c = remainingSum;
                //if(segmentCount > 2) remainingSum *= bendy;
                float C = Geometry.LawOfCosForAngleC(a, b, c);
                segmentVectors[0] = segmentVectors[0].Rotate(C);

                for(int l = 1; l < segments[j][k].Length; l++) {
                    float segmentLength = segmentLengths[j][k][l];
                    
                    segmentVectors[l] = segmentVectors[l].FromRotation(segmentVectors[l - 1], (angleSum - C) / segmentCount);
                    remainingSum -= segmentLength;
                }
                for(int l = 0; l < segments[j][k].Length; l++) {
                    GameObject segment = segments[j][k][l];
                    HingeJoint2D hinge = segment.GetComponent<HingeJoint2D>();

                    float z = 0; if(left) z = 180;
                    if(l == 0) SetHingeAngle(hinge, z + (baseVector.x * (Vector2.SignedAngle(baseVector, segmentVectors[l]) + endpointAngle)));
                    else SetHingeAngle(hinge, baseVector.x * Vector2.SignedAngle(baseVector, segmentVectors[l]));
                }
            }
        }
    }

    Vector2 getEndpoint(int J, int K) { return getEndpoint(J, K, false);  }
    Vector2 getEndpoint(int J, int K, bool forceNew) {
        GameObject leg = legSets[J][K];
        float legLength = legLengths[J][K];
        Vector2 baseVector = Vector2.right;
        if(J%2==0) baseVector = Vector2.left;

        bool withinLength = Vector2.Distance(legSets[J][K].transform.position, endpoints[J][K]) <= legLengths[J][K] * bendy;
        bool withinAngle = Vector2.Angle(baseVector, endpoints[J][K] - (Vector2)leg.transform.position) <= 100;

        forceNew = forceNew || tempEndpoint[J][K];
        if((!withinLength) || (!withinAngle) || forceNew) {
            Vector2 endpoint = endpoints[J][K];
            Vector2 direction = new Vector2(0, 0);
            Vector2 desired = new Vector2(0, 0);
            switch(J) {
                case 0:
                    direction = Vector2.left;
                    desired = Vector2.down;
                    if(!withinLength) {
                        direction = Vector2.down;
                        desired = Vector2.left;
                    }
                    break;
                case 1:
                    direction = Vector2.right;
                    desired = Vector2.down;
                    if(!withinLength) {
                        direction = Vector2.down;
                        desired = Vector2.right;
                    }
                    break;
                case 2:
                    direction = Vector2.up;
                    desired = Vector2.down;
                    break;
            }
            RaycastHit2D hit = Physics2D.Raycast(leg.transform.position, direction, legLength, ~(1 << 8));
            float rotatedTotal = 0.0f;
            while(rotatedTotal < 90.0f && hit.collider == null) {
                direction = direction.RotateTo(desired, rotateAmount);
                rotatedTotal += rotateAmount;
                hit = Physics2D.Raycast(leg.transform.position, direction, legLength * bendy, ~(1 << 8));
            }
            if(hit.collider == null) {
                endpoint = Vector2.down * bendy * legLength + (Vector2)leg.transform.position;
                tempEndpoint[J][K] = true;
            }
            else {
                endpoint = hit.point;
                tempEndpoint[J][K] = false;
            }
            endpoints[J][K] = endpoint;
        }
        return endpoints[J][K];
    }

    void SetHingeAngle(HingeJoint2D hinge, float angle) {
        angle = -angle;
        hinge.useLimits = true;
        JointAngleLimits2D limit = hinge.limits;
        limit.min = angle;
        limit.max = angle;
        hinge.limits = limit;
    }

    GameObject[] getSegmentList(GameObject parent) {
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
                    if(childObj.GetComponent<Rigidbody2D>().mass > 0.0001) Debug.LogWarning("Segment's Rigidbody2d.mass > 0.0001 (minimum mass). Moving legs may cause parent to move, which might cause issues.");
                }
            }
            if(endSegment == oldEndSegment) foundEnd = true;
        }

        return segmentList.ToArray();
    }

    Vector2[] GetSegmentVectors(int J, int K) {
        List<Vector2> vectorList = new List<Vector2>();
        for(int l = 0; l < segments[J][K].Length; l++) {
            GameObject thisSegment = segments[J][K][l], nextSegment;
            Vector2 segmentVector = new Vector2();
            if(l == segments[J][K].Length - 1) {
                segmentVector =
                    (thisSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)thisSegment.transform.position) -
                    (footVectors[J][K]);
            }
            else {
                nextSegment = segments[J][K][l + 1];
                segmentVector =
                    (thisSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)thisSegment.transform.position) -
                    (nextSegment.GetComponent<HingeJoint2D>().anchor + (Vector2)nextSegment.transform.position);
            }
            vectorList.Add(segmentVector);
        }
        return vectorList.ToArray();
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

    void ResetAllLegs() {
        for(int j = 0; j < legSets.Length; j++)
            for(int k = 0; k < legSets[j].Length; k++)
                for(int l = 0; l < segments[j][k].Length; l++)
                    endpoints[j][k] = getEndpoint(j, k, true);
    }
}
