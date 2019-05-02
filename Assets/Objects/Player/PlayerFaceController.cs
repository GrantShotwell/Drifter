using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFaceController : MonoBehaviour {

    #region Variables
    public float displacement = -0.1f;

    private class Parent {
        public GameObject gameObject;
        public Rigidbody2D rigidbody;
        public PlayerController controller;
        public Healthbar healthbar;
    }
    Parent parent = new Parent();

    Animator animator;
    new SpriteRenderer renderer;
    #endregion

    #region Update
    private void Start() {
        parent.gameObject = transform.parent.gameObject;
        parent.rigidbody = parent.gameObject.GetComponent<Rigidbody2D>();
        parent.controller = parent.gameObject.GetComponent<PlayerController>();
        parent.healthbar = parent.gameObject.GetComponent<Healthbar>();
        animator = GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
    }
    
    private void Update() {

        #region Animation
        //- Sets values for the animator component. -//

        //facing
        bool facing = parent.controller.facing;
        animator.SetBool("facing", facing);

        //speed
        float speed = parent.rigidbody.velocity.magnitude;
        animator.SetFloat("speed", speed);

        //horizontalInput
        float horizontalInput = parent.controller.inputs.horizontal;
        animator.SetFloat("horizontalInput", horizontalInput);

        //verticalInput
        float verticalInput = parent.controller.inputs.vertical;
        animator.SetFloat("verticalInput", verticalInput);

        //onGround
        bool onGround = parent.controller.onGround;
        animator.SetBool("onGround", onGround);

        //onWall
        bool onWall = parent.controller.onWall;
        animator.SetBool("onWall", onWall);

        //inAir
        bool inAir = parent.controller.inAir;
        animator.SetBool("inAir", inAir);

        //hurt
        bool hurt = parent.healthbar.invincible;
        animator.SetBool("hurt", hurt);

        //Except for this, which mirrors SpriteRenderer based on 'facing'.
        if(facing && onWall && !onGround) renderer.flipX = true;
        else renderer.flipX = false;
        #endregion

        #region Displacement
        //- Throws the face backwards/forwards in the direction of velocity. -//

        Vector2 offset = parent.rigidbody.velocity;
        float magnitude = offset.magnitude / Time.deltaTime;
        if(magnitude > displacement) offset /= magnitude / displacement;

        if(magnitude != 0) transform.localPosition = offset;
        else transform.localPosition = Vector3.zero;

        #endregion

    }
    #endregion

}
