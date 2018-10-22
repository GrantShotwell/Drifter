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

    [CustomEditor(typeof(NormalRenderer))]
    public class NormalRendererEditor : Editor {
        private SerializedProperty
            _normalSystem,
            _autoDetectNormalSystem,
            _useUpdateForDetection,
            _material;

        private void OnEnable() {
            _normalSystem = serializedObject.FindProperty("normalSystem");
            _autoDetectNormalSystem = serializedObject.FindProperty("autoDetectNormalSystem");
            _useUpdateForDetection = serializedObject.FindProperty("useUpdateForDetection");
            _material = serializedObject.FindProperty("material");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = true;
            _autoDetectNormalSystem.boolValue = EditorGUILayout.ToggleLeft("Automatically find 'NormalSystem' component on a camera.", _autoDetectNormalSystem.boolValue);

            GUI.enabled = true;
            if(_autoDetectNormalSystem.boolValue)
                _useUpdateForDetection.boolValue = EditorGUILayout.ToggleLeft("Use 'Update()' for detection.", _useUpdateForDetection.boolValue);

            GUI.enabled = !_autoDetectNormalSystem.boolValue;
            EditorGUILayout.PropertyField(_normalSystem, new GUIContent("Normal System"));

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_material, new GUIContent("Material"));

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion

    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Material))]
    public class NormalRenderer : MonoBehaviour {
        #region Variables
        public bool autoDetectNormalSystem = true;
        public bool useUpdateForDetection = false;
        public NormalSystem normalSystem;
        public Material material;
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
            material = new Material(GetComponent<SpriteRenderer>().sharedMaterial);
            if(autoDetectNormalSystem && useUpdateForDetection) {
                foreach(Camera camera in Camera.allCameras) {
                    NormalSystem normalSystem = camera.GetComponent<NormalSystem>();
                    if(normalSystem != null) this.normalSystem = normalSystem;
                }
            }

            NormalComponents n = new NormalComponents(0, 0);
            foreach(GameObject light in normalSystem.lights) {
                if(light != null) {
                    NormalSource source = light.GetComponent<NormalSource>();
                    if(source != null) {
                        Vector2 displacement = source.transform.position - transform.position;
                        float distance = displacement.magnitude;
                        n += new NormalComponents(
                            (displacement.y / distance) * (source.radius / distance),
                            (displacement.x / distance) * (source.radius / distance)
                        );
                    }
                    else Debug.LogError("Could not find the 'NormalSource' component of object: " + light);
                }
            }

            material.SetFloat("_Up", n.up);
            material.SetFloat("_Dn", n.dn);
            material.SetFloat("_Rt", n.rt);
            material.SetFloat("_Lt", n.lt);
            GetComponent<SpriteRenderer>().sharedMaterial = material;
        }
    }
}
