using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour{

    public Rigidbody2D body;

    public BoxCollider2D groundCheck;

    public LayerMask groundMask;

    public float acceleration;
    [Range(0f, 1f)]
    public float groundDecay;
    public float maxXSpeed;


    public float jumpSpeed;



    public bool grounded;

    float xInput;

    float yInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update(){
        CheckInput();
        HandleJump();
    }

    void FixedUpdate() {
        CheckGround();
        HandleXMovement();
        ApplyFriction();
    }

    void CheckInput(){
        xInput = Input.GetAxis("Horizontal");
        yInput = Input.GetAxis("Vertical");
    }

    void HandleXMovement(){
        if (Mathf.Abs(xInput) > 0) {

            // zwiekszanie przyspieszenia i prędkości a clamp to żeby się nie zwiększały w nieskończoność
            float increment = xInput * acceleration;
            float newSpeed = Mathf.Clamp(body.velocity.x + increment, -maxXSpeed, maxXSpeed); //zamiast groundspeed jest maXSpeed
            body.velocity = new Vector2(newSpeed, body.velocity.y);

            FaceInput();
        }
    }

    void FaceInput(){
        float direction = Mathf.Sign(xInput);
        transform.localScale = new Vector3(direction, 1, 1);
    }

    // void HandleJump(){
    //     if (Input.GetButtonDown("Jump") && grounded) {
    //         body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    //     }
    // }

    // void HandleJump(){
    //     if (Mathf.Abs(yInput) > 0 && grounded) {
    //         body.velocity = new Vector2(body.velocity.x, yInput * jumpSpeed);
    //     }
    // }

    // void HandleJump(){
    //     if (Input.GetButtonDown("Jump") && grounded) {
    //         body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    //     }
    // }

    void HandleJump() {
    // Sprawdź, czy przycisk "Jump" (spacja) został naciśnięty i czy postać jest na ziemi
    if (Input.GetButtonDown("Jump") && grounded) {
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    }
}

    void CheckGround(){
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    // void ApplyFriction(){
    //    if (grounded && xInput == 0 && yInput == 0 ){
    //         body.velocity *= groundDecay;
    //     } 
    // }

    void ApplyFriction(){
    if (grounded && xInput == 0) {
        body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
    } 
}
}
