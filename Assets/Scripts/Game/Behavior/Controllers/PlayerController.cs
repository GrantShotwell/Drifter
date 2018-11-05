using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyStuff;

#region Player Class
/// <summary>
/// Useful for getting the components of a player (done a lot) without having to do that whole Start() { GetComponent<...>(); } thing.
/// </summary>

[ExecuteInEditMode]
public class PlayerClass : MonoBehaviour {
    public new Rigidbody2D rigidbody;
    public new SpriteRenderer renderer;
    public PlayerController controller;
    public Healthbar healthbar;
    public Rope rope;
    public Puller puller;
    public AimGuidence aimGuidence;

    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        controller = GetComponent<PlayerController>();
        healthbar = GetComponent<Healthbar>();
        rope = GetComponent<Rope>();
        puller = GetComponent<Puller>();
        aimGuidence = GetComponent<AimGuidence>();
    }

    public static implicit operator GameObject(PlayerClass player) { return player.gameObject; }
    public static explicit operator PlayerClass(GameObject player) { return player.GetComponent<PlayerClass>(); }
}
#endregion

[ExecuteInEditMode]
public class PlayerController : MonoBehaviour {
    #region Variables
    public bool EXPLOSIONS = false;
    public GameObject explosion;

    [Tooltip("Layermask for the rope/puller raycast.")]
    public LayerMask ropeLayermask;

    public float groundSpeed;
    public float airSpeed;

    private bool _dead = false;
    public bool dead {
        get { return _dead; }
        private set {
            if(value == true) active = false;
            _dead = value;
        }
    }
    private bool _active = true;
    public bool active {
        get {
            if(dead) return false;
            else return _active;
        }
        private set {
            if(value == true) dead = false;
            _active = value;
        }
    }
    public bool grounded => Physics2D.Raycast(
        transform.position,
        Physics2D.gravity,
        transform.lossyScale.y + 0.1f
    );

    PlayerClass playerClass;
    new Rigidbody2D rigidbody => playerClass.rigidbody;
    new SpriteRenderer renderer => playerClass.renderer;
    Healthbar healthbar => playerClass.healthbar;
    Weapon weapon;
    Rope rope => playerClass.rope;
    Puller puller => playerClass.puller;
    Vector2 mousePos;
    Color originalColor;
    AimGuidence aimGuidence => playerClass.aimGuidence;
    #endregion

    #region Update
    private void Start() {
        if(GetComponent<PlayerClass>() == null) gameObject.AddComponent<PlayerClass>();
        playerClass = GetComponent<PlayerClass>();
        originalColor = renderer.color;
        weapon = GetComponent<Weapon>();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update() {
        if(Application.isPlaying) {
            #region Display
            if(Time.frameCount % 4 <= 2 && healthbar.invincibilityLeft > 0.2) {
                renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
            }
            else {
                renderer.color = originalColor;
            }
            #endregion

            #region Input
            if(active) {
                if(Input.GetButtonDown("Destroy Connection")) {
                    rope.Disconnect();
                    puller.Disconnect();
                }
                if(Input.GetButton("Attack")) {
                    weapon.Fire(Geometry.GetAngle(transform.position, mousePos));
                }

                float horizonalMovement = Input.GetAxis("Horizontal") * Time.deltaTime;
                float verticalMovement = Input.GetAxis("Vertical") * Time.deltaTime;
                Vector2 movement = new Vector2(horizonalMovement, verticalMovement);
                if(grounded) {
                    movement.y = 0;
                    movement *= groundSpeed;
                }
                else movement *= airSpeed;
                transform.Translate(new Vector3(movement.x, movement.y, 0));
            }
            #endregion
        }
    }

    private void LateUpdate() {
        if(Application.isPlaying) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if(active) {
                if(Input.GetButtonDown("Toggle Puller")) {
                    if(puller.isConnected)
                        puller.Disconnect();
                    else {
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, Mathf.Infinity, ropeLayermask.value);
                        if(hit.collider != null && hit.distance <= puller.maxDistance) puller.Connect(hit.point);
                    }
                }
                if(Input.GetButtonDown("Make Rope")) {
                    if(EXPLOSIONS) {
                        GameObject exp = Instantiate(explosion);
                        exp.transform.position = mousePos;
                        //exp.transform.rotation = Quaternion.Euler(0, 0, 90 * (int)Random.Range(0, 3));
                    }
                    else {
                        RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, Mathf.Infinity, ropeLayermask);
                        if(hit.collider != null) rope.Connect(hit.point);
                    }
                }
            }
        }
    }

    private void FixedUpdate() {
        if(Input.GetButton("Shorten Rope")) {
            rope.ShortenRope(rope.retractRate);
        }
    }
    #endregion

    #region Functions
    #region Health
    public void OnDamage() {
        
    }

    public void OnDeath() {
        transform.localScale = new Vector3(0, 0, 0);
        rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        dead = true;
        aimGuidence.hidden = true;
    }

    public void OnReanimate() {
        transform.localScale = new Vector3(1, 1, 1);
        rigidbody.constraints = ~RigidbodyConstraints2D.FreezePosition;
        active = true;
        aimGuidence.hidden = false;
    }
    #endregion
    #endregion
}
