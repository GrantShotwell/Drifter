using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using MyStuff;

[CustomEditor(typeof(RaycastCollision))]
[CanEditMultipleObjects]
public class RaycastCollisionEditor : Editor {
    RaycastCollision script;
    AnimBool
        m_ShowFields1,
        m_ShowFields2;
    SerializedProperty
        _type,
        _offset,
        _radius,
        _box;

    private void OnEnable() {
        script = (RaycastCollision)target;
        m_ShowFields1 = new AnimBool(true);
        m_ShowFields1.valueChanged.AddListener(Repaint);
        m_ShowFields2 = new AnimBool(true);
        m_ShowFields2.valueChanged.AddListener(Repaint);
        _type = serializedObject.FindProperty("type");
        _offset = serializedObject.FindProperty("offset");
        _radius = serializedObject.FindProperty("radius");
        _box = serializedObject.FindProperty("box");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        
        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_type, new GUIContent("Raycast Type"));

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_offset, new GUIContent("Offset From Hit"));

        m_ShowFields1.target = _type.enumValueIndex == 1; // RaycastType.Circle
        if(EditorGUILayout.BeginFadeGroup(m_ShowFields1.faded)) {
            
            /**/ GUI.enabled = true;
            EditorGUILayout.PropertyField(_radius, new GUIContent("Radius"));
            
        }
        EditorGUILayout.EndFadeGroup();

        m_ShowFields2.target = _type.enumValueIndex == 2; // RaycastType.Box
        if(EditorGUILayout.BeginFadeGroup(m_ShowFields2.faded)) {
            
            /**/ GUI.enabled = true;
            EditorGUILayout.PropertyField(_box, new GUIContent("Size"));
            
        }
        EditorGUILayout.EndFadeGroup();

        serializedObject.ApplyModifiedProperties();
    }
}

[RequireComponent(typeof(Rigidbody2D))]
public class RaycastCollision : MonoBehaviour {
    [System.Serializable]
    public enum RaycastType { Ray, Circle, Box }
    public RaycastType type = RaycastType.Ray;
    public float radius, offset;
    public Vector2 box;
    bool inCollision = false;
    new Rigidbody2D rigidbody;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if(!inCollision) {
            RaycastHit2D hit = Raycast();
            if(hit.collider != null && hit.distance > offset) {
                Vector2 hitVector = hit.point - rigidbody.position;
                hitVector = hitVector.SetMagnitude(hitVector.magnitude - offset);
                Debug.DrawLine(rigidbody.position, rigidbody.position + hitVector);
                rigidbody.position += hitVector;
            }
        }
    }

    internal RaycastHit2D Raycast() {
        if(type == RaycastType.Ray) {
            return Physics2D.Raycast(
                rigidbody.position,
                rigidbody.velocity,
                rigidbody.velocity.magnitude * Time.fixedDeltaTime
            );
        } else if(type == RaycastType.Circle) {
            return Physics2D.CircleCast(
                rigidbody.position,
                radius,
                rigidbody.velocity,
                rigidbody.velocity.magnitude * Time.fixedDeltaTime
            );
        } else { //RaycastType.Box
            return Physics2D.BoxCast(
                rigidbody.position,
                box,
                transform.rotation.eulerAngles.z,
                rigidbody.velocity,
                rigidbody.velocity.magnitude * Time.fixedDeltaTime
            );
        }
    }

    private void OnCollisionEnter2D (Collision2D collision)  { inCollision = true;  }
    private void OnCollisionStay2D  (Collision2D collision)  { inCollision = true;  }
    private void OnCollisionExit2D  (Collision2D collision)  { inCollision = false; }
}
