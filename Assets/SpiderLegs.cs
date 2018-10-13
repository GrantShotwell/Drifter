using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class SpiderLegs : MonoBehaviour {
    #region Variables
    public float bendy = 0.8f;

    [System.Serializable]
    public class Leg {
        public GameObject parent;

        [HideInInspector]
        public Segment[] segments;

        [HideInInspector]
        public GameObject foot;

        public Range angle;

        [HideInInspector]
        public float length;

        public bool startFromMin = false;
    }
    public class Segment {
        public GameObject gameObject;
        public HingeJoint2D hinge;
        public float length;
        public float polygonAngle, targetAngle, offsetAngle;
        public bool isEnd;

        public Segment(GameObject GameObj, HingeJoint2D Hinge, bool lastInLeg = false) {
            gameObject = GameObj;
            hinge = Hinge;
            isEnd = lastInLeg;
        }
    }
    public Leg[] legs;
    #endregion

    #region Update
    private void Start() {
        foreach(Leg leg in legs) {
            leg.angle.min = Geometry.NormalizeDegree(leg.angle.min);
            leg.angle.max = Geometry.NormalizeDegree(leg.angle.max);
            leg.segments = GetSegments(leg.parent);
            foreach(Segment segment in leg.segments) {
                if(segment.isEnd) foreach(Transform child in segment.gameObject.transform)
                    if(child.gameObject.name == "Foot Vector") leg.foot = child.gameObject;
                segment.length = MeasureLength(leg, segment);
                segment.offsetAngle = Geometry.NormalizeDegree(MeasureAngle(leg, segment));
            }
        }
    }

    private void Update() {
        // segment.polygonAngle //  The leg segments (+ a line from segments[0].hinge to foot) make a polygon. What are the angles of that polygon?
        foreach(Leg leg in legs) {
            float storedLength = Vector2.Distance(leg.segments[0].gameObject.transform.position, leg.foot.transform.position);
            float storedAngle = 0;
            Geometry.Triangle finalTriangle;
            for(int j = 0; j < leg.segments.Length; j++) {
                Segment segment = leg.segments[j];
                if((!segment.isEnd) && j != leg.segments.Length - 2) {
                    float bendedLength = Vector2.Distance(leg.segments[j + 1].hinge.anchor, leg.foot.transform.position) * bendy;
                    Geometry.Triangle triangle = new Geometry.Triangle(segment.length, storedLength, bendedLength);
                    segment.polygonAngle = triangle.C + storedAngle;
                    storedLength = Vector2.Distance(leg.segments[j + 1].gameObject.transform.position, leg.foot.transform.position);
                    storedAngle = triangle.B;
                } else
                if(j == leg.segments.Length - 2) { //Second to last segment means there are only 2 segments left (duh). Add the storedLength side and that's 3 for the final triangle.
                    finalTriangle = new Geometry.Triangle(segment.length, storedLength, leg.segments[j + 1].length);
                    segment.polygonAngle = finalTriangle.A;
                    leg.segments[j + 1].polygonAngle = finalTriangle.B;
                }
            }
        }

        // segment.targetAngle //  Now that we have all the polygon's information we could ever need, what are the actual angles of the hinges relative to the coordinate axies?
        foreach(Leg leg in legs) {
            for(int j = 0; j < leg.segments.Length; j++) {

            }
        }
    }
    #endregion

    #region Functions
    Segment[] GetSegments(GameObject parent) {
        for(int j = 0; j < legs.Length; j++)
            if(legs[j].parent == parent)
                return GetSegments(j);
        return null;
    }
    Segment[] GetSegments(int legIndex) {
        List<GameObject> objectList = new List<GameObject>();

        GameObject current = legs[legIndex].parent;
        bool foundEnd = false;
        while(!foundEnd) {
            GameObject oldSegment = current;
            foreach(Transform child in oldSegment.transform)
                if(child.gameObject.name == "Segment") current = child.gameObject;
            if(oldSegment == current) foundEnd = true;
            else objectList.Add(current);
        }

        List<Segment> segmentArray = new List<Segment>();
        for(int j = 0; j < objectList.Count; j++) {
            GameObject segment = objectList[j];
            bool last = (j + 1 == objectList.Count);
            segmentArray.Add(new Segment(segment, segment.GetComponent<HingeJoint2D>(), last));
        }

        return segmentArray.ToArray();
    }

    float MeasureLength(Leg leg, Segment segment) {
        int index = -1;
        for(int j = 0; j < leg.segments.Length && index == -1; j++)
            if(leg.segments[j] == segment) index = j;

        if(index >= 0) {
            if(segment.isEnd) return Vector2.Distance(segment.hinge.anchor, leg.foot.transform.position);
            else return Vector2.Distance(segment.hinge.anchor, leg.segments[index + 1].hinge.anchor);
        }
        else {
            Debug.LogError("[Internal Error] Arguments of 'MesureLength(Leg, Segment)' were invalid. The segment could not be found within the given leg.");
            return 0;
        }
    }
    float MeasureLength(Leg leg, bool remeasureSegments = false) {
        if(remeasureSegments)
            foreach(Segment segment in leg.segments)
                segment.length = MeasureLength(leg, segment);
        float sum = 0;
        foreach(Segment segment in leg.segments)
            sum += segment.length;
        return sum;
    }
    
    float MeasureAngle(Leg leg, Segment segment) {
        int index = -1;
        for(int j = 0; j < leg.segments.Length && index == -1; j++)
            if(leg.segments[j] == segment) index = j;

        if(index >= 0) {
            if(segment.isEnd) return Vector2.SignedAngle(segment.hinge.anchor, leg.foot.transform.position);
            else return Vector2.SignedAngle(segment.hinge.anchor, leg.segments[index + 1].hinge.anchor);
        }
        else {
            Debug.LogError("[Internal Error] Arguments of 'MesureAngle(Leg, Segment)' were invalid. The segment could not be found within the given leg.");
            return 0;
        }
    }
    float MeasureAngle(Leg leg) {
        return Geometry.NormalizeDegree(Vector2.SignedAngle(leg.parent.transform.position, leg.foot.transform.position));
    }
    #endregion
}
