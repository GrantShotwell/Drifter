using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyStuff {

    [System.Serializable]
    public class Range {
        public static Range infinite = new Range(Mathf.NegativeInfinity, Mathf.Infinity);

        public float min, max;
        public Range(float minimum = Mathf.NegativeInfinity, float maximum = Mathf.Infinity) {
            min = minimum;
            max = maximum;
        }

        public bool Contains(float value) {
            return min <= value && value <= max;
        }

        public float Place(float value) {
            if(min >= value) value = min;
            if(max <= value) value = max;
            return value;
        }
    }

    [System.Serializable]
    public class Range2D {
        public static Range2D infinite = new Range2D(Range.infinite, Range.infinite);

        public Range x, y;
        public Range2D(Range X, Range Y) {
            x = X;
            y = Y;
        }

        public bool Contains(Vector2 value) {
            return x.Contains(value.x) && y.Contains(value.y);
        }

        public Vector2 Place(Vector2 value) {
            return new Vector2(x.Place(value.x), y.Place(value.y));
        }
    }

    [System.Serializable]
    public class CameraLimit {
        public Range2D position;
        public Range size;
        public bool lerpLimit;
        [ConditionalField("lerpLimit")]
        public float lerp;

        public CameraLimit(Range2D Position = null, Range Size = null) {
            position = Position;
            size = Size;
            lerpLimit = false;
        }

        public CameraLimit(Range2D Position, Range Size, float Lerp) {
            position = Position;
            size = Size;
            lerpLimit = true;
            lerp = Lerp;
        }
    }

    public class TimedAction {
        public Action action;
        public float finalTime, startTime;

        public TimedAction(float delay, Action whenFinished) {
            action = whenFinished;
            if(Time.inFixedTimeStep) finalTime = Time.fixedTime + delay;
            else finalTime = Time.time + delay;
            startTime = finalTime;
        }

        public bool TimerDone() {
            if(Time.inFixedTimeStep) return Time.fixedTime >= finalTime;
            else return Time.time >= finalTime;
        }

        public bool Active() {
            if(Time.inFixedTimeStep) return startTime <= Time.fixedTime && Time.fixedTime <= finalTime;
            else return startTime <= Time.time && Time.time <= finalTime;
        }

        public void Invoke() {
            action();
        }
    }

    public class ContinuousAction : TimedAction {
        public ContinuousAction(float duration, float endDelay, Action duringTime) : base(endDelay, duringTime) {
            if(Time.inFixedTimeStep) startTime = Time.fixedTime + endDelay - duration;
            else startTime = Time.time + endDelay - duration;
        }
    }

    public class TimedActionTracker {
        List<TimedAction> timedActions = new List<TimedAction>();
        bool singleUse;

        public TimedActionTracker(bool deleteWhenDone = true) {
            singleUse = deleteWhenDone;
        }

        public void Update() {
            foreach(TimedAction timedAction in timedActions.ToArray()) {
                if(timedAction.Active()) timedAction.Invoke();
                if(timedAction.TimerDone()) {
                    timedAction.Invoke();
                    if(singleUse) timedActions.Remove(timedAction);
                }
            }
        }

        public void AddAction(TimedAction timedAction) {
            timedActions.Add(timedAction);
        }
    }
}
