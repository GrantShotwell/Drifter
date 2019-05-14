using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using MyStuff;

[CustomEditor(typeof(DripSource))]
[CanEditMultipleObjects]
public class DripSourceEditor : Editor {
    DripSource script;
    SerializedProperty
        _drip,
        _color,
        _max,
        _period,
        _offset;

    private void OnEnable() {
        script = (DripSource)target;
        _drip = serializedObject.FindProperty("drip");
        _color = serializedObject.FindProperty("color");
        _max = serializedObject.FindProperty("max");
        _period = serializedObject.FindProperty("period");
        _offset = serializedObject.FindProperty("offset");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_drip, new GUIContent("Prefab"));

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_color, new GUIContent("Color"));

        /**/ GUI.enabled = !Application.isPlaying;
        EditorGUILayout.PropertyField(_max, new GUIContent("Maximum Drips"));

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_period, new GUIContent("Period"));

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_offset, new GUIContent("Offset"));

        serializedObject.ApplyModifiedProperties();
    }
}

public class DripSource : MonoBehaviour, IDripSource {
    public GameObject drip;
    public float dripSpeed = 0.1f;
    public Color color;
    public int max = 3;
    public float period = 1.0f;
    public float offset = 0.1f;
    public float width = 0.5f;

    private RollingArray<GameObject> drips;
    internal float nextDrip = 0f;

    private void Start() {
        drips = new RollingArray<GameObject>(max, (GameObject item) => { Destroy(item); });
        SetNextDrip();
    }

    private void Update() {

        if(Time.time > nextDrip) {

            GameObject drip = Instantiate(
                this.drip,
                new Vector3(
                    transform.position.x + (width * (Random.value - 0.5f)),
                    transform.position.y + offset,
                    transform.position.z
                ),  transform.rotation
            );

            drip.GetComponent<Drip>().
                source = this;

            drip.GetComponent<SpriteRenderer>().
                color = color;

            drip.GetComponent<Rigidbody2D>().
                velocity = new Vector3(0, -dripSpeed, 0);

            drips.Add(drip);

            SetNextDrip();
        }

    }

    public void DripFreefall(GameObject gameObject) {
        gameObject.GetComponent<Rigidbody2D>().
            isKinematic = false;
    }

    public void DripHitGround(GameObject gameObject) {
        Destroy(gameObject);
    }

    internal void SetNextDrip() { nextDrip = period * (Random.value + 0.5f) + Time.time; }
}
