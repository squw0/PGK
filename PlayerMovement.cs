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

/// <summary>
/// heheszki
/// </summary>
    public bool grounded;

    float xInput;

    float yInput;

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

    void HandleJump() {
    if (Input.GetButtonDown("Jump") && grounded) {
        body.velocity = new Vector2(body.velocity.x, jumpSpeed);
    }
}

    void CheckGround(){
        grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
    }

    void ApplyFriction(){
    if (grounded && xInput == 0) {
        body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
    } 
}
} //BUDDA POZDRAWIAM



// //variable jump height jeżeli puści się spacje to gracz szybciej spada 
// using UnityEngine;

// var fallSpeed = 
//     _endedJumpEarly && _currentVericalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed
// _currentVericalSpeed -= fallSpeedSpeed * Time.deltaTime 


// //Apex modifiers antigravity at the apex of jump and minor speed boost
// _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
// var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
// _currentHorizontalSpeed += apexBonus * Time.deltaTime;

// _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);


// //Jump buffering que next jump before hitting ground
// if(_colDown &&& _lastJumpPressed + _jumpBuffer > Time.time){}

// //Coyote time you can still jump after leaving platform for a couple miliseconds
// if(InputRegistering.JumpDown && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time){
// }

// //Clamped fall speed
// if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp

// //Edge detection you dont hit edges with head player is slighty moved same with ledge catching
// if(_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
// var dir = transform.position - hit.transform.position;
// transform.position += dir.normalized * move.magnitude;















//DO POPRAWEK
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public Rigidbody2D body;
//     public BoxCollider2D groundCheck;
//     public LayerMask groundMask;

//     public float acceleration;
//     [Range(0f, 1f)] public float groundDecay;
//     public float maxXSpeed;
//     public float jumpSpeed;
//     public float fallSpeed;
//     public float jumpEndEarlyGravityModifier = 2f; // Przyspieszenie opadania przy szybkim zakończeniu skoku
//     public float apexBonus = 1.5f; // Dodatkowa prędkość na szczycie skoku
//     public float jumpBufferTime = 0.2f; // Maksymalny czas wyprzedzenia skoku
//     public float coyoteTime = 0.1f; // Maksymalny czas na skok po zejściu z platformy
//     public float maxFallSpeed = -10f; // Maksymalna prędkość opadania

//     private bool grounded;
//     private float xInput;
//     private float yInput;
//     private bool jumpPressed;
//     private bool jumpHeld;

//     private float jumpBufferCounter;
//     private float coyoteTimeCounter;
//     private float currentVerticalSpeed;
//     private float apexPoint; // Wartość odwracająca grawitację na szczycie skoku

//     void Update()
//     {
//         CheckInput();
//         HandleJumpBuffer();
//     }

//     void FixedUpdate()
//     {
//         CheckGround();
//         HandleXMovement();
//         ApplyFriction();
//         HandleVariableJumpHeight();
//         ApplyGravity();
//         ClampFallSpeed();
//     }

//     void CheckInput()
//     {
//         xInput = Input.GetAxis("Horizontal");
//         yInput = Input.GetAxis("Vertical");

//         jumpPressed = Input.GetButtonDown("Jump");
//         jumpHeld = Input.GetButton("Jump");
//     }

//     void HandleXMovement()
//     {
//         if (Mathf.Abs(xInput) > 0)
//         {
//             float increment = xInput * acceleration;
//             float newSpeed = Mathf.Clamp(body.velocity.x + increment, -maxXSpeed, maxXSpeed);
//             body.velocity = new Vector2(newSpeed, body.velocity.y);

//             FaceInput();
//         }
//     }

//     void FaceInput()
//     {
//         float direction = Mathf.Sign(xInput);
//         transform.localScale = new Vector3(direction, 1, 1);
//     }

//     void HandleJumpBuffer()
//     {
//         if (jumpPressed)
//             jumpBufferCounter = jumpBufferTime;

//         if (grounded)
//             coyoteTimeCounter = coyoteTime;
//         else
//             coyoteTimeCounter -= Time.deltaTime;

//         if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
//         {
//             PerformJump();
//             jumpBufferCounter = 0;
//         }
//         else
//             jumpBufferCounter -= Time.deltaTime;
//     }

//     void PerformJump()
//     {
//         currentVerticalSpeed = jumpSpeed;
//         body.velocity = new Vector2(body.velocity.x, currentVerticalSpeed);
//     }

//     void HandleVariableJumpHeight()
//     {
//         if (!jumpHeld && currentVerticalSpeed > 0)
//             currentVerticalSpeed -= fallSpeed * jumpEndEarlyGravityModifier * Time.deltaTime;

//         if (!grounded)
//             apexPoint = Mathf.InverseLerp(jumpSpeed, 0, Mathf.Abs(body.velocity.y));
//     }

//     void ApplyGravity()
//     {
//         float gravityMultiplier = grounded ? 1f : (apexPoint > 0.5f ? 1f : jumpEndEarlyGravityModifier);
//         currentVerticalSpeed -= fallSpeed * gravityMultiplier * Time.deltaTime;

//         body.velocity = new Vector2(body.velocity.x + (Mathf.Sign(xInput) * apexBonus * apexPoint * Time.deltaTime), currentVerticalSpeed);
//     }

//     void ClampFallSpeed()
//     {
//         if (currentVerticalSpeed < maxFallSpeed)
//             currentVerticalSpeed = maxFallSpeed;
//     }

//     void CheckGround()
//     {
//         grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;

//         if (grounded)
//             currentVerticalSpeed = Mathf.Max(currentVerticalSpeed, 0);
//     }

//     void ApplyFriction()
//     {
//         if (grounded && xInput == 0)
//         {
//             body.velocity = new Vector2(body.velocity.x * groundDecay, body.velocity.y);
//         }
//     }
// }


// TEN SUPER ALE COŚ ROZJEBAŁO
// using System.Collections;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     public Rigidbody2D body;
//     public BoxCollider2D groundCheck;
//     public LayerMask groundMask;

//     [Header("INPUT")]
//     [Tooltip("Makes all Input snap to an integer. Prevents gamepads from walking slowly. Recommended value is true to ensure gamepad/keybaord parity.")]
//     public bool SnapInput = true;

//     [Tooltip("Minimum input required before you mount a ladder or climb a ledge. Avoids unwanted climbing using controllers"), Range(0.01f, 0.99f)]
//     public float VerticalDeadZoneThreshold = 0.3f;

//     [Tooltip("Minimum input required before a left or right is recognized. Avoids drifting with sticky controllers"), Range(0.01f, 0.99f)]
//     public float HorizontalDeadZoneThreshold = 0.1f;

//     [Header("MOVEMENT")]
//     [Tooltip("The top horizontal movement speed")]
//     public float MaxSpeed = 14;

//     [Tooltip("The player's capacity to gain horizontal speed")]
//     public float Acceleration = 120;

//     [Tooltip("The pace at which the player comes to a stop")]
//     public float GroundDeceleration = 60;

//     [Tooltip("Deceleration in air only after stopping input mid-air")]
//     public float AirDeceleration = 30;

//     [Tooltip("A constant downward force applied while grounded. Helps on slopes"), Range(0f, -10f)]
//     public float GroundingForce = -1.5f;

//     [Tooltip("The detection distance for grounding and roof detection"), Range(0f, 0.5f)]
//     public float GrounderDistance = 0.05f;

//     [Header("JUMP")]
//     [Tooltip("The immediate velocity applied when jumping")]
//     public float JumpPower = 36;

//     [Tooltip("The maximum vertical movement speed")]
//     public float MaxFallSpeed = 40;

//     [Tooltip("The player's capacity to gain fall speed. a.k.a. In Air Gravity")]
//     public float FallAcceleration = 110;

//     [Tooltip("The gravity multiplier added when jump is released early")]
//     public float JumpEndEarlyGravityModifier = 3;

//     [Tooltip("The time before coyote jump becomes unusable. Coyote jump allows jump to execute even after leaving a ledge")]
//     public float CoyoteTime = .15f;

//     [Tooltip("The amount of time we buffer a jump. This allows jump input before actually hitting the ground")]
//     public float JumpBuffer = .2f;

//     private bool grounded;
//     private float xInput;
//     private bool jumpPressed;
//     private bool jumpHeld;
//     private float jumpBufferCounter;
//     private float coyoteTimeCounter;
//     private float currentVerticalSpeed;
//     private float apexPoint;

//     void Update()
//     {
//         CheckInput();
//         HandleJumpBuffer();
//     }

//     void FixedUpdate()
//     {
//         CheckGround();
//         HandleXMovement();
//         ApplyFriction();
//         HandleVariableJumpHeight();
//         ApplyGravity();
//         ClampFallSpeed();
//     }

//     void CheckInput()
//     {
//         xInput = Input.GetAxis("Horizontal");

//         if (SnapInput) 
//         {
//             xInput = Mathf.Round(xInput);
//         }

//         jumpPressed = Input.GetButtonDown("Jump");
//         jumpHeld = Input.GetButton("Jump");
//     }

//     void HandleXMovement()
//     {
//         if (Mathf.Abs(xInput) > HorizontalDeadZoneThreshold)
//         {
//             float increment = xInput * Acceleration * Time.deltaTime;
//             float newSpeed = Mathf.Clamp(body.velocity.x + increment, -MaxSpeed, MaxSpeed);
//             body.velocity = new Vector2(newSpeed, body.velocity.y);
//             FaceInput();
//         }
//     }

//     void FaceInput()
//     {
//         float direction = Mathf.Sign(xInput);
//         transform.localScale = new Vector3(direction, 1, 1);
//     }

//     void HandleJumpBuffer()
//     {
//         if (jumpPressed)
//             jumpBufferCounter = JumpBuffer;

//         if (grounded)
//             coyoteTimeCounter = CoyoteTime;
//         else
//             coyoteTimeCounter -= Time.deltaTime;

//         if (jumpBufferCounter > 0 && coyoteTimeCounter > 0)
//         {
//             PerformJump();
//             jumpBufferCounter = 0;
//         }
//         else
//             jumpBufferCounter -= Time.deltaTime;
//     }

//     void PerformJump()
//     {
//         currentVerticalSpeed = JumpPower;
//         body.velocity = new Vector2(body.velocity.x, currentVerticalSpeed);
//     }

//     void HandleVariableJumpHeight()
//     {
//         if (!jumpHeld && currentVerticalSpeed > 0)
//             currentVerticalSpeed -= FallAcceleration * JumpEndEarlyGravityModifier * Time.deltaTime;

//         if (!grounded)
//             apexPoint = Mathf.InverseLerp(JumpPower, 0, Mathf.Abs(body.velocity.y));
//     }

//     void ApplyGravity()
//     {
//         float gravityMultiplier = grounded ? 1f : (apexPoint > 0.5f ? 1f : JumpEndEarlyGravityModifier);
//         currentVerticalSpeed -= FallAcceleration * gravityMultiplier * Time.deltaTime;

//         body.velocity = new Vector2(body.velocity.x + (Mathf.Sign(xInput) * apexPoint * Time.deltaTime), currentVerticalSpeed);
//     }

//     void ClampFallSpeed()
//     {
//         if (currentVerticalSpeed < -MaxFallSpeed)
//             currentVerticalSpeed = -MaxFallSpeed;
//     }

//     void CheckGround()
//     {
//         grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;

//         if (grounded)
//             currentVerticalSpeed = Mathf.Max(currentVerticalSpeed, 0);
//     }

//     void ApplyFriction()
//     {
//         if (grounded && Mathf.Abs(xInput) < HorizontalDeadZoneThreshold)
//         {
//             body.velocity = new Vector2(body.velocity.x * GroundDeceleration, body.velocity.y);
//         }
//     }
// }
