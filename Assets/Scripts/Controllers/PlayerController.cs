using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    #region Variables
    [Tooltip("Layermask for the rope/puller raycast.")]
    public LayerMask ropeLayermask;

    public float groundSpeed;
    public float airSpeed;

    public bool dead { private set; get; }

    new Rigidbody2D rigidbody;
    new SpriteRenderer renderer;
    Healthbar healthbar;
    Weapon weapon;
    Rope rope;
    Puller puller;
    Vector2 mousePos;
    Color originalColor;
    #endregion

    #region Update
    private void Start() {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
        originalColor = renderer.color;
        healthbar = GetComponent<Healthbar>();
        weapon = GetComponent<Weapon>();
        rope = GetComponent<Rope>();
        puller = GetComponent<Puller>();
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    private void Update() {
        #region Display
        if(Time.frameCount % 4 <= 2 && healthbar.invincibilityLeft > 0.2) {
            renderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.1f);
        }
        else {
            renderer.color = originalColor;
        }
        #endregion

        #region Input
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
        if(IsGrounded()) {
            movement.y = 0;
            movement *= groundSpeed;
        }
        else movement *= airSpeed;
        transform.Translate(new Vector3(movement.x, movement.y, 0));
        #endregion
    }

    private void LateUpdate() {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetButtonDown("Toggle Puller")) {
            if(puller.isConnected)
                puller.Disconnect();
            else {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, Mathf.Infinity, ropeLayermask.value);
                if(hit.collider != null && hit.distance <= puller.maxDistance) puller.Connect(hit.point);
            }
        }
        if(Input.GetButtonDown("Make Rope")) {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos - (Vector2)transform.position, Mathf.Infinity, ropeLayermask);
            if(hit.collider != null) rope.Connect(hit.point);
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
    }

    public void OnReanimate() {
        transform.localScale = new Vector3(1, 1, 1);
        rigidbody.constraints = ~RigidbodyConstraints2D.FreezePosition;
        dead = false;
    }
    #endregion
    
    public bool IsGrounded() {
        return Physics2D.Raycast(transform.position,
            Physics2D.gravity,
            transform.lossyScale.y + 0.1f
        );
    }
    #endregion
}
