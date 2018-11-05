using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Half made by me, half made by that awesome guy on SO.

[CustomPropertyDrawer(typeof(VectorPickerAttribute))]
public class VectorPickerDrawer : PropertyDrawer {
    #region Variables
    bool _trackMouse;
    SerializedProperty _property;
    MonoBehaviour script;

    ///<summary>Keep the currently selected object to avoid loosing focus while/after tracking</summary>
    GameObject _mySelection;

    ///<summary>For reverting if tracking canceled</summary>
    Vector2 _originalPosition;

    ///<summary>Flag for doing Setup only once</summary>
    bool _setup;

    /// <summary>Mouse position from scene view into the world.</summary>
    Vector2 worldPoint;
    #endregion
    
    /// <summary>
    /// Catch a click event while over the SceneView
    /// </summary>
    /// <param name="sceneView">The current scene view => might not work anymore with multiple SceneViews</param>
    private void UpdateSceneView(SceneView sceneView) {

        Camera cam = SceneView.lastActiveSceneView.camera;
        worldPoint = Event.current.mousePosition;
        worldPoint.y = Screen.height - worldPoint.y - 36.0f; // ??? Why that offset?!
        worldPoint = cam.ScreenToWorldPoint(worldPoint);

        VectorPickerAttribute vectorPicker = attribute as VectorPickerAttribute;
        if(script != null && vectorPicker.relative) worldPoint -= (Vector2)script.transform.position;

        // get current event
        var e = Event.current;

        // Only check while tracking
        if(_trackMouse) {
            if((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && e.button == 0) {
                OnTrackingEnds(false, e);
            }
            else {
                // Prevent losing focus
                Selection.activeGameObject = _mySelection;
            }
        }
        else {
            // Skip if event is Layout or Repaint
            if(e.type == EventType.Layout || e.type == EventType.Repaint) return;

            // Prevent Propagation
            Event.current.Use();
            Event.current = null;

            // Unlock Inspector
            ActiveEditorTracker.sharedTracker.isLocked = false;

            // Prevent losing focus
            Selection.activeGameObject = _mySelection;

            // Remove SceneView callback
            SceneView.onSceneGUIDelegate -= UpdateSceneView;

        }
    }

    /// <summary>
    /// Called when ending Tracking
    /// </summary>
    /// <param name="revert">flag whether to revert to previous value or not</param>
    /// <param name="e">event that caused the ending</param>
    /// <returns>Returns the vector value of the property that we are modifying.</returns>
    private Vector2 OnTrackingEnds(bool revert, Event e) {
        e.Use();
        Event.current = null;
        //Debug.Log("Vector Picker finished");

        if(revert) {
            // restore previous value
            _property.vector2Value = _originalPosition;
            //Debug.Log("Reverted");
        }

        // disable tracking
        _trackMouse = false;

        // Apply changes
        _property.serializedObject.ApplyModifiedProperties();

        return _property.vector2Value;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        script = (MonoBehaviour)property.serializedObject.targetObject;

        if(property.propertyType != SerializedPropertyType.Vector2) {
            EditorGUI.HelpBox(position, "This Attribute requires Vector2", MessageType.Error);
            return;
        }

        var e = Event.current;

        if(!_setup) {
            // store the selected Object (should be the one with this drawer active)
            _mySelection = Selection.activeGameObject;
            _property = property;

            _setup = true;
        }


        // load current value into serialized properties
        _property.serializedObject.Update();

        //specific to the ONE property we are updating
        bool trackingThis = _trackMouse && property.propertyPath == _property.propertyPath;

        GUI.enabled = !trackingThis;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;


        // Write manually changed values to the serialized fields
        _property.serializedObject.ApplyModifiedProperties();



        if(!trackingThis) {
            var button = new Rect(position) {
                x = position.width - 2,
                width = position.height
            };

            // if button wasn't pressed do nothing
            if(!GUI.Button(button, "")) return;

            // store current value in case of revert
            _originalPosition = _property.vector2Value;

            // enable tracking
            _property = property;
            _trackMouse = true;

            // Lock the inspector so we cannot lose focus
            ActiveEditorTracker.sharedTracker.isLocked = true;

            // Prevent event propagation
            e.Use();

            //Debug.Log("Vector Picker started");
            return;
        }

        // <<< This section is only reached if we are in tracking mode >>>

        // Overwrite the onSceneGUIDelegate with a callback for the SceneView
        SceneView.onSceneGUIDelegate = UpdateSceneView;

        // Set to world position
        _property.vector2Value = worldPoint;

        // Track position until either Mouse button 0 (to confirm) or Escape (to cancel) is clicked
        var mouseUpDown = (e.type == EventType.MouseUp || e.type == EventType.MouseDown) && e.button == 0;
        if(mouseUpDown) {
            // End the tracking, don't revert
            property.vector2Value = OnTrackingEnds(false, e);
        }
        else if(e.type == EventType.KeyUp && _trackMouse && e.keyCode == KeyCode.Escape) {
            // Cancel tracking via Escape => revert value
            property.vector2Value = OnTrackingEnds(true, e);
        }

        property.serializedObject.ApplyModifiedProperties();

        //This fixes "randomly stops updating for no reason".
        EditorUtility.SetDirty(property.serializedObject.targetObject);
    }
}
