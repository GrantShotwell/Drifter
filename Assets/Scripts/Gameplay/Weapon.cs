using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {
    #region Variables
    [Header("Weapon Attributes")]

    [Tooltip("Arbitrary name that can be used by other scripts.")]
    public string customName;

    [Tooltip("Decide when 'time' is counted from.")]
    [DefinedValues("Disabled", "Chain From First Fire", "Chain From Last Fire")]
    public string chain = "Disabled";
    [HideInInspector] public bool chainFromFirst = false;
    [HideInInspector] public bool chainFromLast = false;
    [HideInInspector] public bool chainEnabled = true;

    [Tooltip("Amount of time in seconds before the chain resets.")]
    [ConditionalField("chain", "Chain From First Fire")] public float timeFromFirst;
    [Tooltip("Amount of time in seconds before the chain resets.")]
    [ConditionalField("chain", "Chain From Last Fire")] public float timeFromLast;
    [HideInInspector] public float chainLength;

    [Tooltip("Allows a faster rate of fire during a chain.")]
    [ConditionalField("chain", "Chain From First Fire")] public bool canFireWithinChain;
    [Tooltip("Allows a faster rate of fire during a chain.")]
    [ConditionalField("chain", "Chain From Last Fire")] public bool canFireWithinLength;
    [HideInInspector] public bool quickChain;

    [Tooltip("Allows a faster rate of fire during a chain.")]
    [ConditionalField("canFireWithinChain")] public float minimumQuickChainTime;
    [Tooltip("Allows a faster rate of fire during a chain.")]
    [ConditionalField("canFireWithinLength")] public float mininumQuickLengthTime;
    [HideInInspector] public float quickChainRate;

    [Tooltip("Minimum amount of time between each projectile while firing.")]
    public float rateOfFire;

    [Tooltip("Number of projectiles that can be fired before needing to reload.")]
    public int clipSize = 1;

    [Tooltip("Amount of time in seconds it takes to refill the clip.")]
    public float reloadTime;

    [Tooltip("Speed at which the projectile is launched.")]
    public float projectileSpeed = 0.0f;

    [Tooltip("The local position of this weapon where projectiles are summoned.")]
    public Vector2 weaponPosition = new Vector2(0, 0);

    [Header("Projectile Attributes")]
    
    [Tooltip("Prefab GameObjects that are instantiated at the position and angle fired. Attributes like texture, motion, multi-shot, ect. need to be part of the prefab.\n" +
        "Order of 'chain' is the order of the array.\n" +
        "If 'chain' is disabled, a random projectile will be chosen.")]
    public GameObject[] projectiles;
    
    [Tooltip("Projectile's distance away from the center of the weapon. This is affected by angle. '0' means the projectile is instantiated directly on the weapon's position.")]
    public float projectileDisplacement = 0.0f;

    [Tooltip("Change the 'forward' direction of the projectile by an offset of this value.")]
    public float projectileAngle = 0.0f;

    [Tooltip("Attatch a script to the object that will ignore collisions with the parent.")]
    public bool ignoreCollisionsWithParent = true;

    [Tooltip("Instantiate the projectile as a child of the GameObject this script is attached to.")]
    public bool attachToParent = false;

    public bool canFire { get; private set; }
    public float timeSinceFire { get; private set; }
    public float timeOfChain { get; private set; }
    #endregion

    #region Update
    private void Start() {
        switch(chain) {
            case "Disabled": chainEnabled = false;
                break;
            case "Chain From First Fire": chainFromFirst = true;
                chainLength = timeFromFirst;
                quickChain = canFireWithinChain;
                if(quickChain) quickChainRate = minimumQuickChainTime;
                break;
            case "Chain From Last Fire": chainFromLast = true;
                chainLength = timeFromLast;
                quickChain = canFireWithinLength;
                if(quickChain) quickChainRate = mininumQuickLengthTime;
                break;
        }
        chain = null;
    }

    private void FixedUpdate() {
        timeSinceFire += Time.fixedDeltaTime;
        canFire = timeSinceFire >= rateOfFire;
    }
    #endregion

    #region Functions
    public void Fire(float angle) {
        if(canFire || (
            chainEnabled && quickChain
            && timeSinceFire >= quickChainRate //min quick chain length
            && current + 1 >= projectiles.Length //last projectile fired wasn't the last one in the chain
            )
        ) {
            Debug.Log(timeSinceFire);
            canFire = false;
            timeSinceFire = 0;
            ForceFire(angle);
        }
    }

    int current = -1;
    public void ForceFire(float angle) {
        angle += 180;

        GameObject thingToInstantiate;
        if(chainEnabled) {
            float currentTime = Time.time;
            if(Time.inFixedTimeStep) currentTime = Time.fixedTime;

            bool condition = false;
            if(chainFromFirst)
                condition = currentTime >= timeOfChain + (chainLength * current);
            else if(chainFromLast)
                condition = currentTime >= timeOfChain + chainLength;

            if(condition) {
                current++;
                if(current >= projectiles.Length) {
                    current = 0;
                    if(chainFromLast) timeOfChain = currentTime;
                }
            }
            else {
                current = 0;
                timeOfChain = currentTime;
            }

            thingToInstantiate = projectiles[current];
        }
        else thingToInstantiate = projectiles[Random.Range(0, projectiles.Length)];

        GameObject projectile;
        if(attachToParent) projectile = Instantiate(thingToInstantiate, gameObject.transform);
        else projectile = Instantiate(thingToInstantiate);
        Rigidbody2D rigidbody = projectile.GetComponent<Rigidbody2D>();
        projectile.transform.position = Vector2.right.Rotate(angle + projectileAngle) * projectileDisplacement + (Vector2)transform.position + weaponPosition;
        projectile.transform.localRotation = Quaternion.Euler(0, 0, angle + projectileAngle);
        if(rigidbody != null) {
            rigidbody.velocity = Vector2.right.Rotate(angle + projectileAngle) * projectileSpeed;
            if(ignoreCollisionsWithParent) {
                projectile.AddComponent<CollisionIgnorer>().Ignore(gameObject);
            }
        }
    }
    #endregion
}
