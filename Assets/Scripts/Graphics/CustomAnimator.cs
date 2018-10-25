using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using MyStuff;

#region Editor
[CustomEditor(typeof(CustomAnimator))]
public class CustomAnimatorEditor : Editor {
    private SerializedProperty
        _sprites,
        _spriteRenderer,
        _imageRenderer,
        _update,
        _current,
        _cycle,
        _delay,
        _random,
        _catchUp,
        _onFinish,
        _reversed,
        _wait,
        _useComponentEnabler,
        _componentEnabler;

    private void OnEnable() {
        _sprites = serializedObject.FindProperty("sprites");
        _spriteRenderer = serializedObject.FindProperty("spriteRenderer");
        _imageRenderer = serializedObject.FindProperty("imageRenderer");
        _update = serializedObject.FindProperty("update");
        _current = serializedObject.FindProperty("current");
        _cycle = serializedObject.FindProperty("cycle");
        _delay = serializedObject.FindProperty("delay");
        _random = serializedObject.FindProperty("random");
        _catchUp = serializedObject.FindProperty("catchUp");
        _onFinish = serializedObject.FindProperty("onFinish");
        _reversed = serializedObject.FindProperty("reversed");
        _wait = serializedObject.FindProperty("wait");
        _useComponentEnabler = serializedObject.FindProperty("useComponentEnabler");
        _componentEnabler = serializedObject.FindProperty("componentEnabler");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_sprites, new GUIContent("Sprites"), true);
        
        GUI.enabled = true;
        EditorGUILayout.PropertyField(_spriteRenderer, new GUIContent("Sprite Renderer"));

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_imageRenderer, new GUIContent("Image Renderer"));

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_update, new GUIContent("Update Location"));

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_current, new GUIContent("Current Index"));

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_cycle, new GUIContent("Cycle"));

        if(_cycle.intValue == 2) {
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_delay, new GUIContent("Delay"), true);
        }
        
        if(_cycle.intValue != 0) {
            GUI.enabled = true;
            EditorGUILayout.PropertyField(_random, new GUIContent("Randomize"));

            if(!_random.boolValue) {
                if(_cycle.intValue == 2) {
                    GUI.enabled = true;
                    EditorGUILayout.PropertyField(_catchUp, new GUIContent("Catch Up"));
                }

                GUI.enabled = true;
                EditorGUILayout.PropertyField(_onFinish, new GUIContent("On Animation Finish"));

                GUI.enabled = true;
                EditorGUILayout.PropertyField(_reversed, new GUIContent("Reversed"));
            }

            GUI.enabled = true;
            EditorGUILayout.PropertyField(_wait, new GUIContent("Wait"));
        }

        GUI.enabled = true;
        EditorGUILayout.PropertyField(_useComponentEnabler, new GUIContent("Use Component Enabler"));

        GUI.enabled = true;
        if(_useComponentEnabler.boolValue)
            EditorGUILayout.PropertyField(_componentEnabler, new GUIContent("Component Enabler"), true);

        serializedObject.ApplyModifiedProperties();
    }
}

#endregion

public class CustomAnimator : MonoBehaviour {
    #region Variables
    [Tooltip("Array of sprites, in order, for the animator to use.")]
    public Sprite[] sprites;

    [Header("Renderer Component(s)")]

    [Tooltip("Sprite Renderer component of the object to animate. Only one renderer is required.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("UI.Image component of the object to animate. Only one renderer is required.")]
    public UnityEngine.UI.Image imageRenderer;

    [Header("Behavior")]

    [Tooltip("Update on 'FixedUpdate()'. If false, update on 'Update()'. Times, such as 'delay.amount', remain the same.")]
    public UpdateLocation update;

    [Tooltip("Current location in the sprite array. Directly changing this value after 'Start()' will NOT automatically update the sprite without 'UpdateSprite()' unless 'autoCycle' is set to true.")]
    public int current = 0;

    public enum Cycle { None, Frame, Timed }
    public Cycle cycle;
    public bool frame => cycle == Cycle.Frame;
    public bool timed => cycle == Cycle.Timed;

    public bool random = false;

    [System.Serializable]
    public class Delay {
        public enum Period { Frame, Loop }
        public Period period = Period.Frame;
        public bool frame => period == Period.Frame;
        public bool loop  => period == Period.Loop;
        
        [Tooltip("Time, in seconds, of the delay.")]
        public float amount;

        [Tooltip("Randomise 'amount'.")]
        public bool randomize = false;

        [Tooltip("Final amount = [-range, +range].")]
        public float range = 1; //conditional with 'randomise'
    }
    [Tooltip("Decides how long each frame of the animation will last in seconds.")]
    public Delay delay;

    [Tooltip("Skip sprites if the time between updates happened to be much longer than the delay.\n" +
        "Ie. If 'deltaTime >= 2 * delay.amount', then one or more frames will be skipped.")]
    public bool catchUp = true;

    public enum OnFinish { Loop, Reverse, Destroy, Nothing }
    [Tooltip("What to do when 'current' is past the last sprite.")]
    public OnFinish onFinish = 0;
    public bool loop    => onFinish == OnFinish.Loop;
    public bool reverse => onFinish == OnFinish.Reverse;
    public bool destroy => onFinish == OnFinish.Destroy;
    public bool nothing => onFinish == OnFinish.Nothing;
    public bool reversed = false;

    [Tooltip("The animation will be paused as long as this is true.")]
    public bool wait = false;

    public bool useComponentEnabler = false;

    [System.Serializable]
    public class ComponentEnabler {
        public Behaviour component;

        [Header("Behavior")]

        [Tooltip("If false: once disabled, the component will stay disabled.")]
        public bool enableEnabling;

        [Tooltip("If false: once enabled, the component will stay enabled.")]
        public bool enableDisabling;

        [Header("Ranges")]

        [Tooltip("Specify a minimum sprite array location for the component to be enabled.")]
        public int inclusiveMin;

        [Tooltip("Specify a maximum sprite array location for the component to be enabled.")]
        public int inclusiveMax;
    }
    public ComponentEnabler componentEnabler = new ComponentEnabler();

    float time = 0.0f;
    #endregion

    #region Update
    private void Start() {
        SetImage(sprites[current]);
    }

    private void Update() { if(update == UpdateLocation.Update) UpdateAnimation(); }
    private void FixedUpdate() { if(update == UpdateLocation.FixedUpdate) UpdateAnimation(); }
    #endregion

    #region Functions
    private void UpdateAnimation() {
        #region Sprite Cycle
        if(frame && !wait) CycleSprite();
        else if(timed && !wait) {
            if(Time.inFixedTimeStep) time += Time.fixedDeltaTime;
            else time += Time.deltaTime;

            float amount = delay.amount;
            if(delay.randomize)
                amount += Random.Range(-delay.range, delay.range);

            float delayAmount = 0, totalTime = 0;
            if(delay.frame) {
                delayAmount = amount;
                totalTime   = amount * sprites.Length;
            }
            if(delay.loop) {
                delayAmount = amount / sprites.Length;
                totalTime   = amount;
            }

            if(catchUp) {
                float framesToCycle = (current + 1) - (time % delayAmount);
                for(int j = 0; j < framesToCycle; j++) CycleSprite();
            }
            else if(reversed) {
                if(time > -(current + 1) * delayAmount + totalTime) CycleSprite();
            }
            else if(time > (current + 1) * delayAmount) CycleSprite();
        }
        #endregion

        #region Component Enabler
        if(useComponentEnabler) {
            Behaviour component = componentEnabler.component;
            int inclusiveMax = componentEnabler.inclusiveMax, inclusiveMin = componentEnabler.inclusiveMin;
            
            bool componentShouldBeEnabled = true;
            if(current > inclusiveMax)
                componentShouldBeEnabled = false;
            if(current < inclusiveMin)
                componentShouldBeEnabled = false;

            if(component.enabled != componentShouldBeEnabled) {
                if(component.enabled == false && componentEnabler.enableEnabling)
                    component.enabled = true;
                else
                if(component.enabled == true && componentEnabler.enableDisabling)
                    component.enabled = false;
            }
        }
        #endregion
    }

    void CycleSprite() {
        if(random) current = Random.Range(0, sprites.Length);
        else if(current >= sprites.Length - 1 && !reversed) {
            if(destroy) Destroy(gameObject);
            else if(loop) {
                time = 0;
                current = 0;
            }
            else if(reverse) {
                if(!reversed) {
                    time = 0;
                    current = sprites.Length - 2;
                    reversed = true;
                }
            }
        }
        else if(reversed) {
            current--;
            if(current == 0) {
                time = 0;
                reversed = false;
            }

            if(current >= sprites.Length)
                current = 0;
        }
        else current++;
        
        SetImage(sprites[current]);
    }

    public void UpdateSprite() {
        SetImage(sprites[current]);
    }

    void SetImage(Sprite img) {
        if(spriteRenderer != null) spriteRenderer.sprite = img;
        if(imageRenderer != null) imageRenderer.sprite = img;
    }
    #endregion
}
