using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using MyStuff;

namespace Normal2D {
    #region Editor
    /// <summary>
    /// custom inspector stuff
    /// </summary>
    
    [CustomEditor(typeof(NormalSystem))]
    public class NormalSystemEditor : Editor {
        private SerializedProperty
            _lights;

        private void OnEnable() {
            _lights = serializedObject.FindProperty("lights");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            GUI.enabled = false;
            EditorGUILayout.PropertyField(_lights, new GUIContent("Light Sources"), true);

            GUI.enabled = true;
            if(_lights.arraySize == 0)
                EditorGUILayout.HelpBox("No light sources for normal mapping could be found. Make sure that light sources have a 'NormalSource' component.", MessageType.Warning, true);
            
            serializedObject.ApplyModifiedProperties();
        }
    }

    [Serializable]
    public enum LightDetection { Layer, Manual }
    #endregion

    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class NormalSystem : MonoBehaviour {
        #region Variables
        public List<GameObject> lights = new List<GameObject>();
        #endregion
        
        private void Update() {
            lights.Clear();
            foreach(GameObject light in FindObjectsOfType<GameObject>()) {
                NormalSource normalSource = light.GetComponent<NormalSource>();
                if(normalSource != null) lights.Add(light);
            }
        }
    }

    public class NormalComponents {
        public float up, dn, rt, lt;

        public NormalComponents(float vertical, float horizontal) {
            if(vertical > 0) up = vertical;
            else dn = vertical;

            if(horizontal > 0) rt = horizontal;
            else lt = horizontal;

            ClampToBounds();
        }
        public NormalComponents(float up, float down, float right, float left) {
            this.up = up;
            dn = down;
            rt = right;
            lt = left;

            ClampToBounds();
        }

        public void ClampToBounds() {
            Range range = new Range(-1, 1);
            up = range.Place(up);
            dn = range.Place(dn);
            rt = range.Place(rt);
            lt = range.Place(lt);
        }

        public override string ToString() {
            return "[(" + up + ", " + dn + "), (" + rt + ", " + lt + ")]";
        }

        public static NormalComponents operator +(NormalComponents n1, NormalComponents n2) {
            return new NormalComponents(
                n1.up + n2.up,
                n1.dn + n2.dn,
                n1.rt + n2.rt,
                n1.lt + n2.lt
            );
        }

        public static bool operator >(NormalComponents n1, float number) {
            if(n1.up > number) return true;
            if(n1.dn > number) return true;
            if(n1.rt > number) return true;
            if(n1.dn > number) return true;
            return false;
        }
        public static bool operator <(NormalComponents n1, float number) {
            if(n1.up < number) return true;
            if(n1.dn < number) return true;
            if(n1.rt < number) return true;
            if(n1.dn < number) return true;
            return false;
        }
    }
}
