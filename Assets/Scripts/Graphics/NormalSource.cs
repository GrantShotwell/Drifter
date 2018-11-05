using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Light2D {
    #region Editor
    /// <summary>
    /// Custom inspector stuff for 'NormalSource.cs5'.
    /// </summary>
    [CustomEditor(typeof(NormalSource))]
    public class NormalSourceEditor : Editor {
        NormalSource script;
        SerializedProperty
            _falloff,
            _probes;

        private void OnEnable() {
            script = (NormalSource)target;
            _falloff = serializedObject.FindProperty("falloff");
            _probes = serializedObject.FindProperty("probes");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = true;
            EditorGUILayout.PropertyField(_falloff, new GUIContent("Falloff (radius)"));

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_probes, new GUIContent("Affected Probes"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion

    /// <summary>
    /// Component Attached to lights that let NormalRenderer components know where a source is via Rigidbody2D collision.
    /// </summary>
    [RequireComponent(typeof(LightSprite))]
    [RequireComponent(typeof(Collider2D))]
    public class NormalSource : MonoBehaviour {
        #region Variables
        /// <summary>The light source's effect on the material based on distance. (power : distance : : [1, 0] : [0, falloff])</summary>
        public float falloff = 1.0f;

        /// <summary>The NormalRenderer components that this source is affecting.</summary>
        public List<LightProbe> probes = new List<LightProbe>();
        LightProbe probe;
        #endregion

        private void OnTriggerEnter2D(Collider2D collider) {
            probe = collider.gameObject.GetComponent<LightProbe>();
            if(probe != null) {
                probe.lights.Add(this);
                probes.Add(probe);
            }
        }

        private void OnTriggerExit2D(Collider2D collider) {
            probe = collider.gameObject.GetComponent<LightProbe>();
            if(probe != null) {
                probe.lights.Remove(this);
                probes.Remove(probe);
            }
        }

        private void OnDestroy() {
            foreach(LightProbe probe in probes) probe.lights.Remove(this);
        }
    }
}
