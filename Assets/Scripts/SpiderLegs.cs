using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class SpiderLegs : MonoBehaviour {
    #region Variables
    public float bendy = 0.8f;
    public float angleSeparation = 10.0f;

    [System.Serializable]
    public class Leg {
        [Separator]

        public GameObject parent;

        [HideInInspector]
        public Segment[] segments;

        [HideInInspector]
        public GameObject foot;

        public bool relativeAngles = true;

        public AngleRange angleRange;
        public GameObject gameObject;
        public float rotation => gameObject.transform.rotation.eulerAngles.z;
        public AngleRange effectiveRange {
            get {
                if(relativeAngles) return angleRange + rotation;
                else return angleRange;
            }
        }

        public Vector2 defaultPosition;

        [HideInInspector]
        public float length;
        
        public bool leftSide;
        public bool startFromMin => leftSide;

        [HideInInspector]
        public Vector2 targetFoot;

        public Vector2 baseVector => segments[0].hingeVector;
        public float targetAngle => Vector2.SignedAngle(Vector2.right, targetFoot - baseVector);
        [HideInInspector]
        public float totalOffsetAngle = 0;
        [HideInInspector]
        public bool temporaryFooting = false;
    }
    public class Segment {
        public GameObject gameObject;
        public HingeJoint2D hinge;
        public Vector2 hingeVector => hinge.anchor.Rotate(gameObject.transform.rotation.eulerAngles.z) + (Vector2)gameObject.transform.position;
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
            leg.gameObject = gameObject;
            leg.parent.transform.rotation = Quaternion.Euler(0, 0, 0);
            leg.segments = GetSegments(leg.parent);
            foreach(Segment segment in leg.segments) {
                if(segment.isEnd) foreach(Transform child in segment.gameObject.transform)
                    if(child.gameObject.name == "Foot Vector") leg.foot = child.gameObject;
                segment.length = MeasureLength(leg, segment);
                segment.offsetAngle = MeasureAngle(leg, segment) - leg.totalOffsetAngle;
                leg.totalOffsetAngle += segment.offsetAngle;
            }
            leg.length = MeasureLength(leg);
        }
    }

    private void Update() {
        #region Leg Geometry
        /// <summary>
        /// Goal: Angles of all the segments to apply to 'legs[*].segments[*].hinge'
        /// 
        /// Procedure: (everything could be done in one loop, but I think separating it makes it more organized)
        /// Loop 1 - Find where to place the foot based on raycasting between '(Range)leg.angle'.
        /// Loop 2 - Find a polygon for that leg. We can find the angles of that polygot by separating it into multiple triangles and using Geometry.LawOfCos to find the angles.
        /// Loop 3 - We have a polygon. Great. We can finally find the angles of the hinges.
        /// </summary>

        // leg.endpoint //  We need to know where to actually put the leg. Finding this also gives us leg.targetAngle.
        foreach(Leg leg in legs) {
            Debug.DrawRay(leg.baseVector, Vector2.right.Rotate(leg.effectiveRange.min));
            Debug.DrawRay(leg.baseVector, Vector2.right.Rotate(leg.effectiveRange.max));
            Debug.DrawLine(leg.baseVector, leg.targetFoot, Color.cyan);

            float legAngle = Vector2.SignedAngle(Vector2.right, leg.targetFoot - leg.segments[0].hingeVector);
            float legDistance = Vector2.Distance(leg.segments[0].hingeVector, leg.targetFoot);
            AngleRange range = leg.effectiveRange;
            if((!range.Contains(legAngle)) || legDistance > leg.length || leg.temporaryFooting) {
                float start = new float(), end = new float(), deltaAngle = new float();
                bool fromMinimum;
                if(legDistance > leg.length) fromMinimum = !leg.startFromMin;
                else fromMinimum = leg.startFromMin;
                if(fromMinimum) {
                    start = range.min;
                    end = range.max;
                }
                else {
                    start = range.max;
                    end = range.min;
                }
                deltaAngle = - angleSeparation * Geometry.Direction(start - end);
                Vector2 origin = leg.segments[0].hingeVector;
                Vector2 direction = Vector2.right.Rotate(start);
                float currentAngle = start;
                RaycastHit2D hit = Physics2D.Raycast(origin, direction, leg.length * bendy);
                while(hit.collider == null && range.Contains(currentAngle + deltaAngle)) {
                    direction = direction.Rotate(deltaAngle);
                    currentAngle += deltaAngle;
                    hit = Physics2D.Raycast(origin, direction, leg.length * bendy);
                }
                if(hit.collider == null) {
                    leg.targetFoot = leg.defaultPosition + leg.segments[0].hingeVector;
                    leg.temporaryFooting = true;
                }
                else {
                    leg.targetFoot = hit.point;
                    leg.temporaryFooting = false;
                }
            }
        }

        // segment.polygonAngle //  The leg segments (+ a line from segments[0].hinge to foot) make a polygon. What are the angles of that polygon?
        foreach(Leg leg in legs) {
            float storedLength = Vector2.Distance(leg.segments[0].gameObject.transform.position, leg.targetFoot);
            float storedAngle = 0;
            Geometry.Triangle triangle = null;
            for(int j = 0; j < leg.segments.Length - 1; j++) {
                Segment segment = leg.segments[j];
                Segment nextSeg = leg.segments[j + 1];
                if((!segment.isEnd) && (!nextSeg.isEnd)) { //Geometry.Triangle is useful because it determines all of the angles of the triange when it is instantiated when given only the side lengths
                    float bendedLength = Vector2.Distance(nextSeg.hingeVector, leg.targetFoot) * bendy;
                    triangle = new Geometry.Triangle(segment.length, storedLength, bendedLength);
                    segment.polygonAngle = triangle.B + storedAngle;
                    storedLength = Vector2.Distance(nextSeg.gameObject.transform.position, leg.targetFoot);
                    storedAngle = triangle.C;
                } else
                if(j == leg.segments.Length - 2) { //Second to last segment means there are only 2 segments left (duh). Add the storedLength side and that's 3 sides for the final triangle.
                    triangle = new Geometry.Triangle(segment.length, nextSeg.length, storedLength);
                    segment.polygonAngle = triangle.B;
                    nextSeg.polygonAngle = triangle.C;
                }
            }
        }

        // segment.targetAngle //  Now that we have all the polygon's information we could ever need, what are the actual angles of the hinges relative to the coordinate axies (Vector2.right)?
        foreach(Leg leg in legs) {
            for(int j = 0; j < leg.segments.Length; j++) {
                Segment segment = leg.segments[j];
                if(leg.leftSide) segment.polygonAngle = -segment.polygonAngle;
                if(j == 0) segment.targetAngle = segment.polygonAngle + leg.targetAngle - leg.rotation - 360;
                else segment.targetAngle = 180 + segment.polygonAngle;
                SetHingeAngle(segment.hinge, segment.targetAngle + segment.offsetAngle);
            }
        }
        #endregion
    }
    #endregion

    #region Functions
    void SetHingeAngle(HingeJoint2D hinge, float angle) {
        angle = AngleRange.Normalize(-angle);
        hinge.useLimits = true;
        JointAngleLimits2D limit = hinge.limits;
        limit.min = angle;
        limit.max = angle;
        hinge.limits = limit;
    }

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
            if(segment.isEnd) return Vector2.Distance(segment.hingeVector, leg.foot.transform.position);
            else return Vector2.Distance(segment.hingeVector, leg.segments[index + 1].hingeVector);
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
            if(segment.isEnd) return Geometry.GetAngle(segment.hingeVector, leg.foot.transform.position);
            else return Geometry.GetAngle(segment.hingeVector, leg.segments[index + 1].hingeVector);
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

// Graveyard of "I worked too hard on this to delete it but now I realise that I didn't need it"
/* 
 * Vector2 prevSeg = leg.targetFoot - leg.segments[0].hingeVector;
 * Debug.DrawLine(Vector2.zero, prevSeg);
 * Vector2 firstSeg = new Vector2(leg.segments[0].length, 0).Rotate(prevSeg.Heading() + leg.segments[0].polygonAngle);
 * leg.segments[0].targetAngle = firstSeg.Heading();
 * SetHingeAngle(leg.segments[0].hinge, leg.segments[0].targetAngle);
 * prevSeg = firstSeg;
 * 
 * Vector2 currentSegment = new Vector2(segment.length, 0);
 * currentSegment = Geometry.HeadToTailAngle(prevSeg, currentSegment, segment.polygonAngle);
 * currentSegment = currentSegment.SetY(-currentSegment.y);
 * segment.targetAngle = currentSegment.Heading();
 * prevSeg = currentSegment;
 * 
 */
