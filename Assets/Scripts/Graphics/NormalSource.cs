using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Normal2D {
    #region Editor
    /// <summary>
    /// custom inspector stuff
    /// </summary>

    [CustomEditor(typeof(NormalSource))]
    public class NormalSourceEditor : Editor {
        private SerializedProperty
            _normalSystem,
            _autoDetectNormalSystem,
            _useUpdateForDetection,
            _shape,
            _radius;

        private void OnEnable() {
            _normalSystem = serializedObject.FindProperty("normalSystem");
            _autoDetectNormalSystem = serializedObject.FindProperty("autoDetectNormalSystem");
            _useUpdateForDetection = serializedObject.FindProperty("useUpdateForDetection");
            _shape = serializedObject.FindProperty("shape");
            _radius = serializedObject.FindProperty("radius");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = true;
            _autoDetectNormalSystem.boolValue = EditorGUILayout.ToggleLeft("Automatically find 'NormalSystem' component on a camera.", _autoDetectNormalSystem.boolValue);

            GUI.enabled = true;
            if(_autoDetectNormalSystem.boolValue)
                _useUpdateForDetection.boolValue = EditorGUILayout.ToggleLeft("Use 'Update()' for detection. (try to avoid using this)", _useUpdateForDetection.boolValue);

            GUI.enabled = !_autoDetectNormalSystem.boolValue;
            EditorGUILayout.PropertyField(_normalSystem, new GUIContent("Normal System"));

            GUI.enabled = true;
            EditorGUILayout.PropertyField(_shape, new GUIContent("Shape"));

            if(_shape.intValue == 0)
                EditorGUILayout.PropertyField(_radius, new GUIContent("Radius"));
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    [Serializable]
    public enum LightShape { Circle }
    #endregion

    [ExecuteInEditMode]
    [RequireComponent(typeof(Light2D.LightSprite))]
    public class NormalSource : MonoBehaviour {
        #region Variables
        public bool autoDetectNormalSystem = true;
        public bool useUpdateForDetection = false;
        public NormalSystem normalSystem;

        public LightShape shape;
        public float radius = 1.0f;
        public float angle = 10.0f, direction = 0.0f;
        #endregion

        private void Start() {
            if(autoDetectNormalSystem && !useUpdateForDetection) {
                foreach(Camera camera in Camera.allCameras) {
                    NormalSystem normalSystem = camera.GetComponent<NormalSystem>();
                    if(normalSystem != null) this.normalSystem = normalSystem;
                }
            }
        }

        private void Update() {
            if(autoDetectNormalSystem && useUpdateForDetection) {
                foreach(Camera camera in Camera.allCameras) {
                    NormalSystem normalSystem = camera.GetComponent<NormalSystem>();
                    if(normalSystem != null) this.normalSystem = normalSystem;
                }
            }
        }
        
        public NormalComponents ComponentsAtPosition(Vector2 position) {
            float v = 0, h = 0;
            if(shape == LightShape.Circle) {
                h = (transform.position.x - position.x) / radius;
                v = (transform.position.y - position.y) / radius;
            }
            return new NormalComponents(v, h);
        }
    }
}
