using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CustomAnimator : MonoBehaviour {
    #region Variables
    [Tooltip("Array of sprites, in order, for the animator to use.")]
    public Sprite[] sprites;

    [Header("Renderer Components")]

    [Tooltip("Sprite Renderer component of the object to animate. Only one renderer is required.")]
    public SpriteRenderer spriteRenderer;

    [Tooltip("UI.Image component of the object to animate. Only one renderer is required.")]
    public UnityEngine.UI.Image imageRenderer;

    [Header("Behavior")]

    [Tooltip("Update on 'FixedUpdate()'. If false, update on 'Update()'. Times, such as 'delay.amount', remain the same.")]
    public bool fixedUpdate = true;

    [Tooltip("Current location in the sprite array. Directly changing this value after 'Start()' will NOT automatically update the sprite without 'UpdateSprite()' unless 'autoCycle' is set to true.")]
    public int current = 0;

    [Tooltip("Automatically cycle between sprites on a constant time interval. Every [delay] seconds 'current += 1' and the sprite is updated.")]
    public bool timed = false;

    [System.Serializable]
    public class Delay {
        [DefinedValues("Between Frames", "Across Loops")]
        public string period = "Between Frames";
        [HideInInspector] public bool frame;
        [HideInInspector] public bool loop;
        
        [Tooltip("Time, in seconds, of the delay.")]
        public float amount;

        [Tooltip("Randomise 'amount'.")]
        public bool randomise = false;

        [Tooltip("Final amount = [-range, +range].")]
        [ConditionalField("randomise")] public float range = 1;
    }
    [Tooltip("Decides how long each frame of the animation will last in seconds.")]
    [ConditionalField("timed")] public Delay delay;

    [Tooltip("Skip sprites if the time between updates happened to be much longer than the delay.\n" +
        "Ie. If '2 * delay.amount <= [Time.deltaTime/Time.fixedDeltaTime]', then 'current += 1' every [Update()/FixedUpdate()].")]
    [ConditionalField("timed")] public bool catchUp = true;

    [Tooltip("What to do when 'current' is past the last sprite.")]
    [DefinedValues("Nothing", "Loop", "Reverse", "Destroy")]
    public string OnFinish = "Nothing";
    [HideInInspector] public bool loopOnFinish = false;
    [HideInInspector] public bool reverseOnFinish = false;
    private bool reversed = false;
    [HideInInspector] public bool destroyOnFinish = false;

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

        [Tooltip("Specify a maximum sprite array location for the component to be enabled. +Infinity if false.")]
        public bool useInclusiveMax;
        [ConditionalField("useInclusiveMax")] public int inclusiveMax;

        [Tooltip("Specify a minimum sprite array location for the component to be enabled. -Infinity if false.")]
        public bool useInclusiveMin;
        [ConditionalField("useInclusiveMin")] public int inclusiveMin;
    }
    [ConditionalField("useComponentEnabler")] public ComponentEnabler componentEnabler = new ComponentEnabler();

    float time = 0.0f;
    #endregion

    #region Update
    private void Start() {
        switch(OnFinish) {
            case "Loop":
                loopOnFinish = true;
                break;
            case "Reverse":
                reverseOnFinish = true;
                break;
            case "Destroy":
                destroyOnFinish = true;
                break;
        }
        OnFinish = null;

        switch(delay.period) {
            case "Between Frames": delay.frame = true;
                break;
            case "Across Loops": delay.loop = true;
                break;
        }
        delay.period = null;

        //TODO: REMOVE THIS
        if(reverseOnFinish)
            catchUp = false;

        SetImage(sprites[current]);
    }

    private void Update() { if(!fixedUpdate) UpdateAnimation(); }
    private void FixedUpdate() { if(fixedUpdate) UpdateAnimation(); }
    #endregion

    #region Functions
    private void UpdateAnimation() {
        #region Sprite Cycle
        if(timed && !wait) {
            if(fixedUpdate) time += Time.fixedDeltaTime;
            else time += Time.deltaTime;

            float amount = delay.amount;
            if(delay.randomise)
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
        else if(!wait) CycleSprite();
        #endregion

        #region Component Enabler
        if(useComponentEnabler) {
            Behaviour component = componentEnabler.component;
            bool useInclusiveMax = componentEnabler.useInclusiveMax, useInclusiveMin = componentEnabler.useInclusiveMin;
            int inclusiveMax = componentEnabler.inclusiveMax, inclusiveMin = componentEnabler.inclusiveMin;

            bool componentShouldBeEnabled = true;
            if(useInclusiveMax && current > inclusiveMax)
                componentShouldBeEnabled = false;
            if(useInclusiveMin && current < inclusiveMin)
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
        if(current >= sprites.Length - 1 && !reversed) {
            if(destroyOnFinish) {
                Destroy(gameObject);
            }
            else if(loopOnFinish) {
                time = 0;
                current = 0;
            }
            else if(reverseOnFinish) {
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

    static int CountTrue(params bool[] args) { return args.Count(t => t); } //Stack Overflow
    #endregion
}
