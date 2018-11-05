using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MyStuff;

namespace Light2D {
    #region Editor
    /// <summary>
    /// Custom inspector stuff for 'NormalRenderer.cs'.
    /// </summary>
    [CustomEditor(typeof(NormalRenderer))]
    [CanEditMultipleObjects]
    public class NormalRendererEditor : Editor {
        NormalRenderer script;
        SerializedProperty
            _normal,
            _probe;

        private void OnEnable() {
            script = (NormalRenderer)target;
            _normal = serializedObject.FindProperty("normal");
            _probe = serializedObject.FindProperty("probe");
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_normal, new GUIContent("Normal Map"));

            GUI.enabled = true;
            EditorGUILayout.PropertyField(_probe, new GUIContent("Light2D Probe"));

            serializedObject.ApplyModifiedProperties();
        }
    }
    #endregion
    
    /// <summary>
    /// Class for the component that passes data to the material about the direction of incoming light(s).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Material))]
    public class NormalRenderer : MonoBehaviour {
        #region Variables
        /// <summary>The normal texture that is applied to the material.</summary>
        public Texture normal;

        /// <summary>The LightProbe related to this NormalRenderer.</summary>
        public LightProbe probe;

        /// <summary>The list of light sources that are affecting the material.</summary>
        public List<NormalSource> lights => probe.lights;

        private MaterialPropertyBlock material;
        private new SpriteRenderer renderer;
        #endregion

        private void Start() {
            material = new MaterialPropertyBlock();
            renderer = GetComponent<SpriteRenderer>();
        }

        private void Update() {
            var n = new NormalComponents(0, 0);
            foreach(NormalSource source in lights) {
                Vector2 displacement = source.transform.position - transform.position;
                float distance = displacement.magnitude;
                n += new NormalComponents(
                    (displacement.y / distance) * (source.falloff / distance),
                    (displacement.x / distance) * (source.falloff / distance)
                );
            }
            
            float rotation = transform.rotation.eulerAngles.z;
            if(rotation > 0) n.Rotate(180 - rotation);

            renderer.GetPropertyBlock(material);
            material.SetFloat("_Up", n.up);
            material.SetFloat("_Dn", n.dn);
            material.SetFloat("_Rt", n.rt);
            material.SetFloat("_Lt", n.lt);
            material.SetTexture("_Normal", normal);
            renderer.SetPropertyBlock(material);
        }
    }

    /// <summary>
    /// Class used for containing data about direction of incoming 2D light sources from multiple directions.
    /// </summary>
    public class NormalComponents {
        /// <summary>The amount of light coming from above.</summary>
        public float up;

        /// <summary>The amount of light coming from below.</summary>
        public float dn;

        /// <summary>The amount of light coming from the right.</summary>
        public float rt;

        /// <summary>The amount of light coming from the left.</summary>
        public float lt;

        /// <summary>Creates NormalComponents from a direction vector.</summary>
        /// <param name="direction">The direction of incoming light.</param>
        /// <param name="clamp">Clamp the up/dn/rt/lt values to [-1, 1]?</param>
        public NormalComponents(Vector2 direction, bool clamp = true) : this(direction.y, direction.x, clamp) { }

        /// <summary>Creates NormalComponents from turning vertical/horizontal values into up/down/left/right values.</summary>
        /// <param name="vertical">The Y component of the incoming light direction vector.</param>
        /// <param name="horizontal">The X component of the incoming light direction vector.</param>
        /// <param name="clamp">Clamp the up/dn/rt/lt values to [-1, 1]?</param>
        public NormalComponents(float vertical, float horizontal, bool clamp = true) {
            if(vertical > 0) up = vertical;
            else dn = vertical;

            if(horizontal > 0) rt = horizontal;
            else lt = horizontal;

            if(clamp) ClampToBounds();
        }

        /// <summary>Creates new NormalComponents from up/dn/rt/lt values.</summary>
        /// <param name="up">The amount of light coming from above.</param>
        /// <param name="down">The amount of light coming from below.</param>
        /// <param name="right">The amount of light coming from the right.</param>
        /// <param name="left">The amount of light coming from the left.</param>
        /// <param name="clamp">Clamp the up/dn/rt/lt values to [-1, 1]?</param>
        public NormalComponents(float up, float down, float right, float left, bool clamp = true) {
            this.up = up;
            dn = down;
            rt = right;
            lt = left;

            if(clamp) ClampToBounds();
        }

        /// <summary>Clamps each component to [-1, 1].</summary>
        public void ClampToBounds() {
            Range range = new Range(-1, 1);
            up = range.Place(up);
            dn = range.Place(dn);
            rt = range.Place(rt);
            lt = range.Place(lt);
        }

        public void Rotate(float degrees) {
            Vector2[] components = {
                Vector2.down * up,
                Vector2.down * dn,
                Vector2.left * rt,
                Vector2.left * lt
            };

            var n = new NormalComponents(0, 0);
            foreach(Vector2 c in components) {
                float rad = degrees * Mathf.Deg2Rad;
                float sin = Mathf.Sin(rad), cos = Mathf.Cos(rad);
                n += new NormalComponents(
                    (sin * c.x) + (cos * c.y),
                    (cos * c.x) - (sin * c.y)
                );
            }
            
            up = n.up;
            dn = n.dn;
            rt = n.rt;
            lt = n.lt;
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

        public static explicit operator NormalComponents(Vector2 v) {
            return new NormalComponents(Vector2.ClampMagnitude(v, 1));
        }

        public static explicit operator Vector2(NormalComponents n) {
            return new Vector2(n.rt + n.lt, n.up + n.dn);
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
