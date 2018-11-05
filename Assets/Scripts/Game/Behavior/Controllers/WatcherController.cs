using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

public class WatcherController : MonoBehaviour {
    #region Variables
    public GameObject target;
    public GameObject[] explosions;
    public LayerMask layerMask;

    #region Attacks
    [System.Serializable]
    public class Attacks {
        public GameObject muncher;

        public float timer = 0.0f;
        public float delay = 5.0f;
        public float groundTime = 1.0f;
        public int current = 0;

        public Color pinkyBombColor;
        public Color lazerColor;
    }
    public Attacks attack = new Attacks();
    #endregion

    #region Eye
    [System.Serializable]
    public class Eye {
        [Header("GameObjects")]
        public GameObject parent;
        public GameObject light;
        public GameObject iris;
        public GameObject irisLight;
        public GameObject lid0, lid1;

        [Header("Iris Settings")]
        public Color defaultColor;
        public Color currentColor;
        public float irisSpeed;
        public float radius;

        [Header("Lid Settings")]
        public Vector2 lidClosed;
        public Vector2 lidOpened;
        public float lidSpeed;
        public int lidState;

        [HideInInspector]
        public Vector2
            irisTarget = new Vector2(),
            lid0Target = new Vector2(),
            lid1Target = new Vector2();
        [HideInInspector]
        public bool rotate = true;
        public void ResetColor() { currentColor = defaultColor; }
    }
    public Eye eye = new Eye();
    #endregion

    #region Misc.
    float defaultHeight;
    float defaultFollow;
    float defaultDrag;
    new SpriteRenderer renderer;
    Hover hoverScript;
    SpiderLegs legScript;
    Follow followScript;
    Healthbar healthbar;
    TimedActionTracker timedActions = new TimedActionTracker();
    new Rigidbody2D rigidbody;
    Weapon pinkyBombWeapon;
    Weapon lazerWeapon;
    #endregion
    #endregion

    #region Update
    private void Start() {
        renderer = GetComponent<SpriteRenderer>();
        attack.muncher.SetActive(false);
        healthbar = GetComponent<Healthbar>();
        healthbar.enabled = false;
        hoverScript = GetComponent<Hover>();
        hoverScript.enabled = false;
        hoverScript.layerMask = layerMask;
        defaultHeight = hoverScript.height;
        legScript = GetComponent<SpiderLegs>();
        legScript.enabled = false;
        legScript.layerMask = layerMask;
        followScript = GetComponent<Follow>();
        followScript.enabled = false;
        defaultFollow = followScript.minimumDistance;
        rigidbody = GetComponent<Rigidbody2D>();
        defaultDrag = rigidbody.drag;
        foreach(Weapon weapon in GetComponents<Weapon>()) {
            switch(weapon.customName) {
                case "Pinky Bomb": pinkyBombWeapon = weapon; break;
                case "Lazer": lazerWeapon = weapon; break;
            }
        }
    }

    bool keepRotation;
    private void Update() {
        timedActions.Update();
        if(healthbar.invincible) {
            if(Time.frameCount % 10 < 5) GetComponent<SpriteRenderer>().color = Color.red;
            else GetComponent<SpriteRenderer>().color = Color.white;
        }
        else GetComponent<SpriteRenderer>().color = Color.white;

        #region Eye
        eye.light.SetActive(eye.lidState == 1);
        eye.irisLight.SetActive(eye.lidState == 1);

        #region Lids
        switch(eye.lidState) {
            case 0: //closed
                eye.lid0Target =  eye.lidClosed;
                eye.lid1Target = -eye.lidClosed;
                break;
            case 1: //open
                eye.lid0Target =  eye.lidOpened;
                eye.lid1Target = -eye.lidOpened;
                break;
        }
        
        eye.lid0.transform.position = Vector2.Lerp(eye.lid0.transform.position, eye.lid0Target.Rotate(eye.parent.transform.rotation.eulerAngles.z) + (Vector2)eye.parent.transform.position, eye.lidSpeed);
        eye.lid1.transform.position = Vector2.Lerp(eye.lid1.transform.position, eye.lid1Target.Rotate(eye.parent.transform.rotation.eulerAngles.z) + (Vector2)eye.parent.transform.position, eye.lidSpeed);
        #endregion

        #region Iris
        eye.irisTarget = target.transform.position;

        if(eye.rotate) {
            float angleToTarget = Geometry.GetAngle(eye.irisTarget, eye.iris.transform.position);
            eye.iris.transform.position += (Vector3)(Vector2.right.Rotate(angleToTarget) * eye.irisSpeed * Time.deltaTime);
            float angleToEye = Geometry.GetAngle(eye.parent.transform.position, eye.iris.transform.position);
            if(eye.irisLight.activeSelf)
                eye.irisLight.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angleToEye + 90));

            float distanceFromEye = Vector2.Distance(eye.parent.transform.position, eye.iris.transform.position);
            if(distanceFromEye > eye.radius) {
                float distanceOverRadius = distanceFromEye - eye.radius;
                eye.iris.transform.position += (Vector3)(Vector2.right.Rotate(angleToEye) * distanceOverRadius);
            }
        }
        #endregion

        #region Color
        eye.irisLight.GetComponent<Light2D.LightSprite>().Color = eye.currentColor;
        eye.light.GetComponent<Light2D.LightSprite>().Color = eye.currentColor;
        eye.parent.GetComponent<SpriteRenderer>().color = new Color(eye.currentColor.r, eye.currentColor.g, eye.currentColor.b, 1);
        #endregion
        #endregion
        
        #region Height and Rotation
        hoverScript.height = defaultHeight + Mathf.Cos(Time.time);
        float targetRotation = Mathf.Sin(Time.time);
        if(keepRotation) {
            float rotation = transform.rotation.eulerAngles.z;
            if(rotation > 180) rotation -= 360;
            rigidbody.AddTorque((targetRotation - rotation - rigidbody.angularVelocity) / Time.deltaTime, ForceMode2D.Force);
        }
        #endregion

        #region Attacks
        if(active) {
            attack.timer += Time.deltaTime;
            if(playerOnGround && Time.time + attack.groundTime > timeOnGround) {
                followScript.minimumDistance = 0.0f;
                if(Mathf.Abs(target.transform.position.x - transform.position.x) < 0.5f) {
                    rigidbody.drag = 0.0f;
                    rigidbody.velocity = new Vector2(0.0f, rigidbody.velocity.y);
                    hoverScript.enabled = false;
                    followScript.enabled = false;
                    attack.muncher.SetActive(true);
                }
                else {
                    rigidbody.drag = defaultDrag;
                    hoverScript.enabled = true;
                    followScript.enabled = true;
                    attack.muncher.SetActive(false);
                }
            }
            else if(attack.timer > attack.delay) {
                switch(attack.current) {
                    case 0:
                        AttackInfo pinkyBomb = new AttackInfo(0.0f, 2.0f, 2.0f);
                        attack.timer = -pinkyBomb.total;
                        attack.current++;
                        timedActions.AddAction(new ContinuousAction(pinkyBomb.delay, pinkyBomb.charge, "attack", () => {
                            eye.currentColor = Color.Lerp(eye.defaultColor, attack.pinkyBombColor, (attack.timer + pinkyBomb.total) / pinkyBomb.charge);
                        }));
                        timedActions.AddAction(new TimedAction(pinkyBomb.total, "attack", () => {
                            eye.ResetColor();
                            int count = 6;
                            float deltaAngle = 360 / count;
                            float angle = 0;
                            while(count > 0) {
                                pinkyBombWeapon.ForceFire(angle);
                                angle += deltaAngle;
                                count--;
                            }
                        }));
                        break;
                    case 1:
                        AttackInfo lazer = new AttackInfo(0.9f, 0.1f, 1.0f);
                        attack.timer = -lazer.total;
                        attack.current++;
                        timedActions.AddAction(new ContinuousAction(lazer.delayedCharge, "attack", () => {
                            eye.currentColor = Color.Lerp(eye.defaultColor, attack.lazerColor, (attack.timer + lazer.total) / lazer.delayedCharge);
                        }));
                        timedActions.AddAction(new ContinuousAction(lazer.delay, lazer.charge, "attack", () => {
                            followScript.enabled = false;
                            eye.rotate = false;
                        }));
                        timedActions.AddAction(new TimedAction(lazer.total, "attack", () => {
                            followScript.enabled = true;
                            eye.rotate = true;
                            eye.ResetColor();
                        }));
                        break;
                    default:
                        attack.timer = 0;
                        attack.current++;
                        break;
                }
            }
            if(attack.current > 1) attack.current = 0;
            if(!playerOnGround) {
                rigidbody.drag = defaultDrag;
                hoverScript.enabled = true;
                followScript.enabled = true;
                attack.muncher.SetActive(false);
                followScript.minimumDistance = defaultFollow;
            }
        }
        #endregion
    }
    #endregion

    #region Functions
    bool active = false;
    public void Activate() {
        eye.lidState = 1;
        timedActions.AddAction(new TimedAction(1, "awake", () => {
            hoverScript.enabled = true;
            legScript.enabled = true;
        }));
        timedActions.AddAction(new TimedAction(2, "awake", () => {
            rigidbody.AddTorque(-360, ForceMode2D.Force);
        }));
        timedActions.AddAction(new ContinuousAction(1, 1, "awake", () => {
            if(180 < transform.rotation.eulerAngles.z) keepRotation = true;
        }));
        timedActions.AddAction(new TimedAction(3, "awake", () => {
            healthbar.enabled = true;
            followScript.enabled = true;
            keepRotation = true;
            active = true;
        }));
    }

    private bool playerOnGround = false;
    private float timeOnGround = 0.0f;
    public void PlayerOnGround(bool state) {
        playerOnGround = state;
        if(Time.inFixedTimeStep) timeOnGround = Time.fixedDeltaTime;
        else timeOnGround = Time.time;
    }

    public void OnDamage() {
        int count = 10;
        Range distance = new Range(1.0f, 3.0f);
        while(count > 0) {
            float magnitude = distance.random;
            Vector2 p = new Vector2(magnitude, 0).Rotate(Random.Range(0.0f, 360.0f));
            GameObject explosion = Instantiate(MyFunctions.RandomItem(explosions));
            explosion.transform.position = transform.position + (Vector3)p;
            explosion.GetComponent<Rigidbody2D>().velocity = p.SetMagnitude(p.magnitude * 4);
            count--;
        }
    }

    public int catLives = 0;
    public Sprite stage2Sprite;
    public void OnDeath() {
        if(catLives == 0) {

        }
        else {
            renderer.sprite = stage2Sprite;
            healthbar.Heal(healthbar.max);
            catLives--;
            
            int count = 30;
            Range distance = new Range(1.0f, 3.0f);
            while(count > 0) {
                float magnitude = distance.random;
                Vector2 p = new Vector2(magnitude, 0).Rotate(Random.Range(0.0f, 360.0f));
                GameObject explosion = Instantiate(MyFunctions.RandomItem(explosions));
                explosion.transform.position = transform.position + (Vector3)p;
                explosion.GetComponent<Rigidbody2D>().velocity = p.SetMagnitude(p.magnitude * 4);
                explosion.GetComponent<SpriteRenderer>().sortingOrder = 10;
                count--;
            }
        }
    }
    #endregion

    #region Other Classes
    private class AttackInfo {
        public float delay, charge, delayedCharge, wait, total;
        public AttackInfo(float delay, float charge, float wait) {
            this.delay = delay;    //Time to wait before charging
            this.charge = charge;  //Time it takes to charge, then fire
            this.wait = wait;      //Time to wait after firing
            total = delay + charge + wait;
            delayedCharge = delay + charge;
        }
    }
    #endregion
}
