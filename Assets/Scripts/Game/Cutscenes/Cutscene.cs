using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Cutscene : MonoBehaviour {
    [ReadOnly] public bool active = false;
    [System.Serializable]
    public class Events {
        public UnityEvent onActive;
        public UnityEvent onFinish;
    }
    public Events events = new Events();

    private void Start() { if(active) During(); }

    public void Activate() {
        active = true;
        events.onActive.Invoke();
    }

    public void Deactivate() {
        active = false;
        events.onFinish.Invoke();
    }

    public abstract void Begin();
    public abstract void During();
    public abstract void Finish();
}
