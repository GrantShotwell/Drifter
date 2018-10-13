using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class SpiderLegs : MonoBehaviour {
    #region Variables
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
                segment.length = MeasureLength(leg, segment);
                segment.offsetAngle = Geometry.NormalizeDegree(MeasureAngle(leg, segment));
            }
        }
    }

    private void Update() {
        foreach(Leg leg in legs) {
            for(int j = 0; j < legs.Length; j++) {

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
