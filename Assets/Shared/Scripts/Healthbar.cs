using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;
using UnityEditor.AnimatedValues;

#region Editor   
[CustomEditor(typeof(Healthbar))]
[CanEditMultipleObjects]
public class HealthbarEditor : Editor {
    Healthbar script;
    AnimBool m_ShowFields1;
    SerializedProperty
        _max,
        _invincibilityTime,
        _events;

    private void OnEnable() {
        script = (Healthbar)target;
        m_ShowFields1 = new AnimBool(true);
        m_ShowFields1.valueChanged.AddListener(Repaint);
        _max = serializedObject.FindProperty("max");
        _invincibilityTime = serializedObject.FindProperty("invincibilityTime");
        _events = serializedObject.FindProperty("events");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();

        /**/ GUI.enabled = false;
        EditorGUILayout.IntField(new GUIContent("Health"), script.health);

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_max, new GUIContent("Max Health"));

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_invincibilityTime, new GUIContent("Invincibility Time"));

        /**/ GUI.enabled = false;
        EditorGUILayout.Toggle(new GUIContent("Invincible"), script.invincible);

        /**/ GUI.enabled = false;
        EditorGUILayout.IntField(new GUIContent("Overflow"), script.overflow);

        /**/ GUI.enabled = true;
        EditorGUILayout.PropertyField(_events, new GUIContent("Events"));

        serializedObject.ApplyModifiedProperties();
    }
}
#endregion

public class Healthbar : MonoBehaviour {
    #region Variables
    [Tooltip("Maximum health value allowed.")]
    public int max = 3;

    [Tooltip("Amount of time in seconds that Damage() is not functional after Damage() is called.")]
    public float invincibilityTime = 2;
    public float invincibilityLeft { get; private set; }

    [System.Serializable]
    public class Events {
        [Tooltip("Event called when health is changed by Damage() or when health <= 0 and Damage() is called.")]
        public UnityEvent damage;

        [Tooltip("Event called when the health value after Damage() is <= 0. This includes when no damage is delt from invincibility.")]
        public UnityEvent death;

        [Tooltip("Event called when health reaches maximum value when Heal() is called.")]
        public UnityEvent healed;
    }
    public Events events = new Events();

    public int health { get; private set; }
    public int overflow { get; private set; }
    public float lastTimeChanged { get; private set; }
    public bool invincible => invincibilityLeft > 0;
    #endregion

    #region Update
    private void Start() {
        health = max;
    }

    private void Update() {

    }

    private void FixedUpdate() {
        if(invincibilityLeft > 0) invincibilityLeft -= Time.fixedDeltaTime;
    }
    #endregion

    #region Functions
    public void Damage(int amount) {
        if(invincibilityLeft <= 0) {
            invincibilityLeft = invincibilityTime;
            health -= amount;
            if(health < 0) {
                overflow += health;
                health = 0;
            }
            else overflow = 0;
            lastTimeChanged = Time.time;
            events.damage.Invoke();
        }
        if(health <= 0) events.death.Invoke();
    }

    public void Heal(int amount) {
        health += amount;
        if(health > max) {
            overflow += health - max;
            health = max;
            lastTimeChanged = Time.time;
            events.healed.Invoke();
        }
        else overflow = 0;
    }
    
    public void Set(int amount) {
        health += amount;
        bool reachedMax = false, reachedMin = false;
        if(health > max) {
            overflow += health - max;
            health = max;
            reachedMax = true;
        }
        if(health < 0) {
            overflow += health;
            health = 0;
            reachedMin = true;
        }
        if((!reachedMin) && (!reachedMax)) overflow = 0;
    }
    #endregion
}
