using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Healthbar : MonoBehaviour {
    #region Variables
    [Tooltip("Maximum health value allowed.")]
    public int max = 3;

    [Tooltip("Amount of time in seconds that Damage() is not functional after Damage() is called.")]
    public float invincibilityTime = 2;
    public float invincibilityLeft { get; private set; }

    [System.Serializable]
    public class Info {
        [Tooltip("The readonly health value. Clamped from [0, max]. Can only be changed with GetComponent<Healthbar>.[Damage(int)/Heal(int)/Set(int)]")]
        [ReadOnly] public int health = 0;

        [Tooltip("If Damage()/Heal()/Set() brings the health above/below the minimum/maximum values, then what is left goes into overflow. This value is reset when health goes the \"other way\".")]
        [ReadOnly] public int overflow = 0;

        [Tooltip("Time left of invincibility")]
        [ReadOnly] public int invincibilityLeft;
    }
    [Tooltip(" - - - INSPECTOR ONLY - - - \n" +
        "To access in script: GetComponent<Healthbar>.[health/overflow/...]")]
    public Info info = new Info();

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
    #endregion

    #region Update
#if UNITY_EDITOR
    private void Start() {
        health = max;
        info.health = health;
        info.overflow = overflow;
    }
#endif

    private void Update() {
        info.health = health;
        info.overflow = overflow;
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
