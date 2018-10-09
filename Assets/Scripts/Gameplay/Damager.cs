using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Damager : MonoBehaviour {
    #region Variables
    [Header("Behavior")]

    public int damage = 1;

    public LayerMask layers;

    [Header("Detection")]

    public bool useCollision;
    [System.Serializable]
    public class ColliderSettings {
        public bool damageOnEnter = true;
        public bool damageOnStay = true;
        public bool damageOnExit = false;
    }
    [ConditionalField("useCollision")] public ColliderSettings colliderSettings = new ColliderSettings();

    public bool useTrigger;
    [System.Serializable]
    public class TriggerSettings {
        public bool damageOnEnter = true;
        public bool damageOnStay = true;
        public bool damageOnExit = false;
    }
    [ConditionalField("useTrigger")] public TriggerSettings triggerSettings = new TriggerSettings();

    [System.Serializable]
    public class DamageEvent : UnityEvent<Vector2> {}

    [Header("Events")]
    public DamageEvent damageEvent;
    #endregion

    #region Update
    #region Collision
    private void OnCollisionEnter2D(Collision2D collision) {
        if(useCollision && colliderSettings.damageOnEnter) {
            collisionActivated(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision) {
        if(useCollision && colliderSettings.damageOnStay) {
            collisionActivated(collision);
        }
    }

    private void OnCollisionExit2D(Collision2D collision) {
        if(useCollision && colliderSettings.damageOnExit) {
            collisionActivated(collision);
        }
    }
    #endregion

    #region Trigger
    private void OnTriggerEnter2D(Collider2D collider) {
        if(useTrigger && triggerSettings.damageOnEnter) {
            triggerActivated(collider);
        }
    }

    private void OnTriggerStay2D(Collider2D collider) {
        if(useTrigger && triggerSettings.damageOnStay) {
            triggerActivated(collider);
        }
    }

    private void OnTriggerExit2D(Collider2D collider) {
        if(useTrigger && triggerSettings.damageOnExit) {
            triggerActivated(collider);
        }
    }
    #endregion
    #endregion

    #region Functions
    void collisionActivated(Collision2D collision) {
        bool ignore = false;
        
        if(layers != (layers | (1 << collision.gameObject.layer))) //thanks StackOverflow
            ignore = true;

        if(!ignore) {
            Healthbar healthbar = collision.gameObject.GetComponent<Healthbar>();
            if(healthbar != null) {
                healthbar.Damage(damage);
                damageEvent.Invoke(collision.transform.position);
            }
        }
    }

    void triggerActivated(Collider2D collision) {
        bool ignore = false;
        if(!ignore) {
            Healthbar healthbar = collision.gameObject.GetComponent<Healthbar>();
            if(healthbar != null) {
                healthbar.Damage(damage);
                damageEvent.Invoke(collision.transform.position);
            }
        }
    }
    #endregion
}
