using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Light2D
{
    public class LightingSystemCreationWindow : EditorWindow {
        private int _lightSourcesLayer;
        private int _lightProbesLayer;
        private int _lightObstaclesLayer;
        private int _ambientLightLayer;

        public static void CreateWindow() {
            var window = GetWindow<LightingSystemCreationWindow>("Lighting system creation window");
            window.position = new Rect(200, 200, 500, 140);
        }

        void OnGUI() {
            if (FindObjectOfType<LightingSystem>()) {
                GUILayout.Label("WARNING: existing lighting system is found.\nIt is recommended to remove it first, before adding new one.", EditorStyles.boldLabel);
            }

            GUILayout.Label("Select layers you wish to use. You could modify them later in created object.");
            _lightSourcesLayer = EditorGUILayout.LayerField("Light Sources", _lightSourcesLayer);
            _lightProbesLayer = EditorGUILayout.LayerField("Light Probes", _lightProbesLayer);
            _lightObstaclesLayer = EditorGUILayout.LayerField("Light Obstacles", _lightObstaclesLayer);
            _ambientLightLayer = EditorGUILayout.LayerField("Ambient Light", _ambientLightLayer);

            if (GUILayout.Button("Create")) {
                var mainCamera = Camera.main;
                var lightingSystem = mainCamera.GetComponent<LightingSystem>() ?? mainCamera.gameObject.AddComponent<LightingSystem>();

                var prefab = Resources.Load<GameObject>("Lighting Camera");
                var lightingSystemObj = Instantiate(prefab);
                lightingSystemObj.name = lightingSystemObj.name.Replace("(Clone)", "");
                lightingSystemObj.transform.parent = mainCamera.transform;
                lightingSystemObj.transform.localPosition = Vector3.zero;
                lightingSystemObj.transform.localScale = Vector3.one;
                lightingSystemObj.transform.localRotation = Quaternion.identity;

                var config = lightingSystemObj.GetComponent<LightingSystemPrefabConfig>();

                lightingSystem.LightCamera = lightingSystemObj.GetComponent<Camera>();
                lightingSystem.AmbientLightComputeMaterial = config.AmbientLightComputeMaterial;
                lightingSystem.LightOverlayMaterial = config.LightOverlayMaterial;
                lightingSystem.AmbientLightBlurMaterial = lightingSystem.LightSourcesBlurMaterial = config.BlurMaterial;

                DestroyImmediate(config);

                lightingSystem.LightCamera.depth = mainCamera.depth - 1;

                lightingSystem.LightCamera.cullingMask = 1 << _lightSourcesLayer;

                lightingSystem.LightSourcesLayer = _lightSourcesLayer;
                lightingSystem.LightProbesLayer = _lightProbesLayer;
                lightingSystem.LightObstaclesLayer = _lightObstaclesLayer;
                lightingSystem.AmbientLightLayer = _ambientLightLayer;

                mainCamera.cullingMask &=
                    ~((1 << _lightSourcesLayer) | (1 << _ambientLightLayer) | (1 << _lightObstaclesLayer));

                Close();
            }
        }
    }
}
