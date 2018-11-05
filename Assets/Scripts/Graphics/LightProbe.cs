using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Light2D {
    #region Editor
    /// <summary>
    /// Custom editor stuff for 'LightProbe.cs'.
    /// </summary>
    [CustomEditor(typeof(LightProbe))]
    [CanEditMultipleObjects]
    public class LightProbeEditor : Editor {
        LightProbe script;
        SerializedProperty
            _lights;

        private void OnEnable() {
            script = (LightProbe)target;
            _lights = serializedObject.FindProperty("lights");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_lights, new GUIContent("Detected Lights"), true);

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion

    /// <summary>
    /// Component used for detecting light sources.
    /// </summary>
    [RequireComponent(typeof(Collider2D))]
    public class LightProbe : MonoBehaviour {
        #region Variables
        /// <summary>All of the NormalSources that are in contact with this probe.</summary>
        public List<NormalSource> lights = new List<NormalSource>();
        #endregion
    }
}
