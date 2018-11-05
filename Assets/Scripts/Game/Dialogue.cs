using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyStuff;

#region Editor
[CustomEditor(typeof(Dialogue))]
public class DialogueEditor : Editor {
    Dialogue script;
    SerializedProperty
        _segments,
        _running,
        _current;

    private void OnEnable() {
        script = (Dialogue)target;
        _segments = serializedObject.FindProperty("segments");
        _running = serializedObject.FindProperty("running");
        _current = serializedObject.FindProperty("current");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUI.enabled = (!script.running) && Application.isPlaying;
        if(GUILayout.Button("Run"))
            script.Run();

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_segments, new GUIContent("Segments"), true);

        serializedObject.ApplyModifiedProperties();
    }
}
#endregion

public class Dialogue : MonoBehaviour {
    #region Variables
    /// <summary>The segments of the dialogue sequence.</summary>
    public Segment[] segments;

    /// <summary>Is the dialogue sequence in progress?</summary>
    public bool running { get; private set; } = false;

    /// <summary>What was the last instantiated index of a Segment in the 'segments' array?</summary>
    public int current { get; private set; } = 0;

    /// <summary>
    /// A class for a segment of the dialogue sequence.
    /// </summary>
    [System.Serializable]
    public class Segment {
        /// <summary>The related Dialogue component.</summary>
        [HideInInspector] public Dialogue parent;

        /// <summary>The text that the 'CustomText' is given when instantiated.</summary>
        public string text;

        /// <summary>The time it takes to say this word, and move on to the next.</summary>
        public float length;

        /// <summary>The position that the text is instantiated at.</summary>
        [VectorPicker(relative: true)] public Vector2 position;

        /// <summary>The refrence 'CustomText' component (AKA prefab).</summary>
        public CustomText Component;

        /// <summary>Instantiated 'CustomText' component. Defaults to 'Component' (prefab) when null.</summary>
        public CustomText component {
            get {
                if(gameObject != null) return gameObject.GetComponent<CustomText>();
                else return Component;
            }
            set { Component = value; }
        }

        /// <summary>The instantiated 'CustomText' GameObject.</summary>
        public GameObject gameObject;

        /// <summary>Is the text highlighted?</summary>
        public bool highlighted {
            get { return component.highlighted; }
            set { component.highlighted = value; }
        }

        /// <summary>Instantiate the 'CustomText' of this segment with it's current values.</summary>
        /// <returns>Returns this segment when finished.</returns>
        public Segment Create() {
            gameObject = Instantiate(component.gameObject, parent.gameObject.transform);
            gameObject.transform.localPosition = position;
            gameObject.GetComponent<CustomText>().UpdateText(text);
            return this;
        }
    }
    #endregion

    #region Update
    private void Start() {
        foreach(Segment segment in segments)
            segment.parent = this;
    }

    float storedTime = 0.0f;
    private void Update() {
        if(running) {
            if(Time.time > storedTime + segments[current].length) {
                segments[current].highlighted = false;
                current++;
                if(current >= segments.Length) Stop();
                else {
                    segments[current].Create().highlighted = true;
                    storedTime = MyFunctions.time;
                }
            }
        }
    }
    #endregion

    #region Functions
    /// <summary>Begins the dialogue sequence.</summary>
    public void Run() {
        Clear();
        running = true;
        current = 0;

        if(segments[0] != null) {
            segments[0].Create().highlighted = true;
        }
        storedTime = MyFunctions.time;
    }

    /// <summary>Ends the dialogue sequence.</summary>
    public void Stop() {
        running = false;
    }

    /// <summary>Destroys all children.</summary>
    public void Clear() {
        foreach(Segment segment in segments) {
            Destroy(segment.gameObject);
        }
    }
    #endregion
}
