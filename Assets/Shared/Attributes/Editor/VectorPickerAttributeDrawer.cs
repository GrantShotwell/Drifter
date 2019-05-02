using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//Half made by me, half made by that awesome guy on SO.

[CustomPropertyDrawer(typeof(VectorPickerAttribute))]
public class VectorPickerDrawer : PropertyDrawer {
    #region Variables
    bool trackMouse;
    SerializedProperty stored;
    MonoBehaviour script;

    ///<summary>Keep the currently selected object to avoid loosing focus while/after tracking</summary>
    GameObject selection;

    ///<summary>For reverting if tracking canceled</summary>
    Vector2 originalPosition;

    ///<summary>Flag for doing Setup only once</summary>
    bool firstFrame = true;

    /// <summary>Mouse position from scene view into the world.</summary>
    Vector2 worldPoint;
    #endregion
    
    /// <summary>
    /// Catch a click event while over the SceneView.
    /// </summary>
    /// <param name="sceneView">The current scene view => might not work anymore with multiple SceneViews</param>
    private void UpdateSceneView(SceneView sceneView) {
        // finds mouse -> world point
        Camera cam = SceneView.lastActiveSceneView.camera;
        worldPoint = Event.current.mousePosition;
        worldPoint.y = Screen.height - worldPoint.y - 36.0f;
        worldPoint = cam.ScreenToWorldPoint(worldPoint);

        // make the worldPoint relative, if enabled
        VectorPickerAttribute vectorPicker = attribute as VectorPickerAttribute;
        if(script != null && vectorPicker.relative) worldPoint -= (Vector2)script.transform.position;
        
        var e = Event.current;
        if(trackMouse) {
            if((e.type == EventType.MouseDown || e.type == EventType.MouseUp) && e.button == 0) {
                OnTrackingEnds(false, e);
            }
            else {
                // Prevent losing focus
                Selection.activeGameObject = selection;
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
            Selection.activeGameObject = selection;

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
        e.Use(); //stops other things from getting left click
        Event.current = null;
        trackMouse = false;
        if(revert) stored.vector2Value = originalPosition;
        return stored.vector2Value;
    }

    /// <summary>
    /// The function called to draw the property.
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        script = (MonoBehaviour)property.serializedObject.targetObject;
        if(property.propertyType != SerializedPropertyType.Vector2) {
            EditorGUI.HelpBox(position, "This Attribute requires Vector2.", MessageType.Error);
            return;
        }

        Event e = Event.current;
        bool trackingThis = trackMouse && property.propertyPath == stored.propertyPath;

        if(firstFrame) {
            selection = Selection.activeGameObject;
            stored = property;
            firstFrame = false;
        }
        
        //Draw the property on the instector.
        GUI.enabled = !trackingThis;
        EditorGUI.PropertyField(position, property, label);
        GUI.enabled = true;

        if(!trackingThis) {
            Rect button = new Rect(position) { x = position.width - 2, width = position.height };
            if(GUI.Button(button, "")) {
                originalPosition = stored.vector2Value;
                stored = property; //sets this property as the one being tracked
                trackMouse = true;
                ActiveEditorTracker.sharedTracker.isLocked = true;
                e.Use(); //stops other things from getting the left click
            }
        }

        if(trackingThis) {
            SceneView.onSceneGUIDelegate = UpdateSceneView;
            stored.vector2Value = worldPoint;

            // Track position until either Mouse button 0 (to confirm) or Escape (to cancel) is clicked
            bool mouseButton0 = (e.type == EventType.MouseUp || e.type == EventType.MouseDown) && e.button == 0;
            bool escKey = e.type == EventType.KeyUp && trackMouse && e.keyCode == KeyCode.Escape;
            if(mouseButton0) property.vector2Value = OnTrackingEnds(false, e);
            else if(escKey) property.vector2Value = OnTrackingEnds(true, e);

            //This fixes "randomly stops updating for no reason".
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }
}
