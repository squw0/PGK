using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody2D body;
    public AudioClip deathSound; // Assign this in the Inspector
    private AudioSource audioSource; // Reference to the AudioSource component
    public int health;
    
    public float moveSpeed;
    public float jumpSpeed;
    public int jumpLeft;
    public Vector2 climbJumpForce;
    public float fallSpeed;
    public float sprintSpeed;
    public float sprintTime;
    public float sprintInterval;
    public float attackInterval;

    public Color invulnerableColor;
    public Vector2 hurtRecoil;
    public float hurtTime;
    public float hurtRecoverTime;
    public Vector2 deathRecoil;
    public float deathDelay;

    public Vector2 attackUpRecoil;
    public Vector2 attackForwardRecoil;
    public Vector2 attackDownRecoil;

    public GameObject attackUpEffect;
    public GameObject attackForwardEffect;
    public GameObject attackDownEffect;

    private bool _isGrounded;
    private bool _isClimb;
    private bool _isSprintable;
    private bool _isSprintReset;
    private bool _isInputEnabled;
    private bool _isFalling;
    private bool _isJumping; // Dodaj tę flagę
    private bool _isAttackable;
    private bool _isDead; // Track if the player is dead

    private float _climbJumpDelay = 0.2f;
    private float _attackEffectLifeTime = 0.05f;

    private Rigidbody2D _rigidbody;
    private Transform _transform;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;
    public BoxCollider2D groundCheck; // Assign this in the Inspector
    public LayerMask groundMask; // Assign this in the Inspector

    private void Start()
    {
        _isDead = false;
        _isJumping = false; // Inicjalizacja flagi
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from the player!");
        }

        _isInputEnabled = true;
        _isSprintReset = true;
        _isAttackable = true;
        _isDead = false;

        _rigidbody = gameObject.GetComponent<Rigidbody2D>();
        _transform = gameObject.GetComponent<Transform>();
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (_isDead) return; // Stop updating if the player is dead

        updatePlayerState();
        if (_isInputEnabled)
        {
            move();
            jumpControl();
        }
    }

    private void updatePlayerState()
    {
        _isGrounded = checkGrounded();
        Variables.Application.Set("IsGround", _isGrounded);

        float verticalVelocity = _rigidbody.velocity.y;
        Variables.Application.Set("IsDown", verticalVelocity < 0);

        if (_isGrounded && verticalVelocity == 0)
        {
            Variables.Application.Set("IsJump", false);
            CustomEvent.Trigger(gameObject, "ResetJumpTriggers");
            jumpLeft = 2;
            _isClimb = false;
            _isSprintable = true;
            _isJumping = false;
        }
        else if (_isClimb)
        {
            jumpLeft = 1;
        }
    }

    private void move()
    {
        float horizontalMovement = Input.GetAxis("Horizontal") * moveSpeed;
        Vector2 newVelocity = new Vector2(horizontalMovement, _rigidbody.velocity.y);
        _rigidbody.velocity = newVelocity;

        anim.SetFloat("speed", Mathf.Abs(horizontalMovement));

        if (horizontalMovement != 0)
        {
            FaceInput(horizontalMovement);
        }
    }

    void FaceInput(float horizontalMovement)
    {
        float direction = Mathf.Sign(horizontalMovement);
        Vector3 newScale = transform.localScale;
        newScale.x = Mathf.Abs(newScale.x) * direction;
        transform.localScale = newScale;
    }

    private void jumpControl()
    {
        if (!Input.GetButtonDown("Jump")) return;
        if (_isJumping) return;

        if (_isClimb)
            climbJump();
        else if (jumpLeft > 0)
            jump();
    }

    private void jump()
    {
        Vector2 newVelocity = new Vector2(_rigidbody.velocity.x, jumpSpeed);
        _rigidbody.velocity = newVelocity;

        anim.SetTrigger("jump");
        jumpLeft -= 1;
        _isJumping = true;
    }

    private void climbJump()
    {
        Vector2 realClimbJumpForce = new Vector2(climbJumpForce.x * transform.localScale.x, climbJumpForce.y);
        _rigidbody.AddForce(realClimbJumpForce, ForceMode2D.Impulse);

        CustomEvent.Trigger(gameObject, "IsClimbJump");
        CustomEvent.Trigger(gameObject, "IsJumpFirst");

        _isInputEnabled = false;
        StartCoroutine(climbJumpCoroutine(_climbJumpDelay));
        _isJumping = true;
    }

    private bool checkGrounded()
    {
        // Vector2 origin = _transform.position;
        // float radius = 0.2f;
        // Vector2 direction = Vector2.down;
        // float distance = 0.5f;
        // LayerMask groundMask = LayerMask.GetMask("Platform");

        // RaycastHit2D hit = Physics2D.CircleCast(origin, radius, direction, distance, groundMask);
        // return hit.collider != null;
        //grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
        // Use OverlapBox to check for ground collision
        Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.bounds.center, groundCheck.bounds.size, 0f, groundMask);
        return colliders.Length > 0;
    }

    private IEnumerator climbJumpCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        _isInputEnabled = true;
        CustomEvent.Trigger(gameObject, "ResetClimbJumpTrigger");

        Vector3 newScale = new Vector3(-transform.localScale.x, 1, 1);
        transform.localScale = newScale;
    }

    public void hurt(int damage)
    {
        if (_isDead) return; // Prevent taking damage if already dead

        health -= damage;

        if (health <= 0)
        {
            die();
        }

        Debug.Log("Player took " + damage + " damage! Health remaining: " + health);
    }
    private void die()
    {
        _isDead = true;
        _isInputEnabled = false;

        // Powiadom kamerę, że gracz umarł
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.OnPlayerDeath();
        }

        // Play death animation
        if (anim != null)
        {
            anim.SetTrigger("Die"); // Wyzwól animację śmierci
        }
        else
        {
            Debug.LogWarning("Animator component missing from the player!");
        }

        // Play death sound
        if (deathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(deathSound);
        }
        else
        {
            Debug.LogWarning("Death sound or AudioSource not set!");
        }

        // Disable player movement and collision
        DisablePlayer();

        // Wait for the death animation and sound to finish before showing the death screen
        StartCoroutine(WaitForDeathAnimationAndSound());
    }

    public bool IsAlive()
    {
        return !_isDead; // _isDead to flaga, która jest ustawiana na true, gdy gracz umiera
    }

    private void DisablePlayer()
    {
        // Disable movement and other scripts
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (var script in scripts)
        {
            if (script != this) // Don't disable this script
            {
                script.enabled = false;
            }
        }

        // Disable Rigidbody
        if (_rigidbody != null)
        {
            _rigidbody.velocity = Vector2.zero;
            _rigidbody.isKinematic = true;
        }

        // Disable Collider
        if (_boxCollider != null)
        {
            _boxCollider.enabled = false;
        }
    }
    private IEnumerator WaitForDeathAnimationAndSound()
    {
    // Poczekaj na zakończenie animacji śmierci
    if (anim != null)
    {
        // Poczekaj, aż Animator przejdzie do stanu "Death"
        yield return WaitForAnimatorState("Death");

        // Pobierz długość animacji śmierci
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        float animationLength = stateInfo.length;

        // Poczekaj na zakończenie animacji
        yield return new WaitForSeconds(animationLength);
    }
    else
    {
        // Jeśli nie ma animatora, poczekaj domyślny czas (np. 1 sekunda)
        yield return new WaitForSeconds(1f);
    }

    // Poczekaj na zakończenie dźwięku śmierci
    if (deathSound != null)
    {
        yield return new WaitForSeconds(deathSound.length);
    }

    // Przejdź do ekranu śmierci
    ShowDeathScreen();
    }

    private IEnumerator WaitForAnimatorState(string stateName)
    {
        // Czekaj, aż Animator przejdzie do stanu o nazwie "stateName"
        while (!anim.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null; // Poczekaj na następną klatkę
        }
    }
    private void ShowDeathScreen()
    {
        // Znajdź GameManager i wywołaj metodę gameOver
        GameManagerScript gameManager = FindObjectOfType<GameManagerScript>();
        if (gameManager != null)
        {
            gameManager.gameOver();
        }
        else
        {
            Debug.LogWarning("GameManagerScript not found in the scene!");
        }
    }
}





// MOVEMENT 1

// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;

// public class PlayerMovement : MonoBehaviour{

//     public Animator anim;
//     public Rigidbody2D body;

//     public BoxCollider2D groundCheck;

//     public LayerMask groundMask;

//     public float acceleration;
//     [Range(0f, 1f)]
//     public float groundDecay;
//     public float maxXSpeed;

//     public float jumpSpeed;

//     public bool grounded;

//     float xInput;

//     float yInput;

//     void Update(){
//         CheckInput();
//         HandleJump();
//     }

//     void FixedUpdate() {
//         CheckGround();
//         HandleXMovement();
//         ApplyFriction();
//         HandleAnimation();        
//     }

//     private void HandleAnimation()
//     {
//         anim.SetFloat("speed", Mathf.Abs(body.linearVelocityX));
//     }

//     void CheckInput(){
//         xInput = Input.GetAxis("Horizontal");
//         yInput = Input.GetAxis("Vertical");
//     }

//     void HandleXMovement(){
//         if (Mathf.Abs(xInput) > 0) {

//             // zwiekszanie przyspieszenia i prędkości a clamp to żeby się nie zwiększały w nieskończoność
//             float increment = xInput * acceleration;
//             float newSpeed = Mathf.Clamp(body.linearVelocity.x + increment, -maxXSpeed, maxXSpeed); //zamiast groundspeed jest maXSpeed
//             body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);

//             FaceInput();
//         }
//     }

//     void FaceInput(){
//         float direction = Mathf.Sign(xInput);
//         transform.localScale = new Vector3(direction, 1, 1);
//     }

//     void HandleJump() {
//         if (Input.GetButtonDown("Jump") && grounded) {
//             body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
//             anim.SetTrigger("jump");
//         }
//     }

//     void CheckGround(){
//         grounded = Physics2D.OverlapAreaAll(groundCheck.bounds.min, groundCheck.bounds.max, groundMask).Length > 0;
//     }

//     void ApplyFriction(){
//     if (grounded && xInput == 0) {
//         body.linearVelocity = new Vector2(body.linearVelocity.x * groundDecay, body.linearVelocity.y);
//     } 
// }
// } 























































































































// MOVEMENT 2

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




























// MOVEMENT 3

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



















// MOVEMENT 3

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
//     public float groundDecay;
//     //public float maxXSpeed;

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
//             float newSpeed = Mathf.Clamp(body.linearVelocity.x + increment, -MaxSpeed, MaxSpeed);
//             body.linearVelocity = new Vector2(newSpeed, body.linearVelocity.y);
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
//         body.linearVelocity = new Vector2(body.linearVelocity.x, currentVerticalSpeed);
//     }

//     void HandleVariableJumpHeight()
//     {
//         if (!jumpHeld && currentVerticalSpeed > 0)
//             currentVerticalSpeed -= FallAcceleration * JumpEndEarlyGravityModifier * Time.deltaTime;

//         if (!grounded)
//             apexPoint = Mathf.InverseLerp(JumpPower, 0, Mathf.Abs(body.linearVelocity.y));
//     }

//     void ApplyGravity()
//     {
//         float gravityMultiplier = grounded ? 1f : (apexPoint > 0.5f ? 1f : JumpEndEarlyGravityModifier);
//         currentVerticalSpeed -= FallAcceleration * gravityMultiplier * Time.deltaTime;

//         body.linearVelocity = new Vector2(body.linearVelocity.x + (Mathf.Sign(xInput) * apexPoint * Time.deltaTime), currentVerticalSpeed);
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
//         // if (grounded && Mathf.Abs(xInput) < HorizontalDeadZoneThreshold)
//         if (grounded && xInput == 0)
//         {
//             body.linearVelocity = new Vector2(body.linearVelocity.x * GroundDeceleration, body.linearVelocity.y);
//         }
//     }
// }




// MOVEMENT 4

// using UnityEngine;

// public class PlayerMovement : MonoBehaviour
// {
//     //public Animator animator;
//     public Rigidbody2D body;
//     public BoxCollider2D groundCheck;
//     public LayerMask groundMask;

//     [Header("STEROWANIE")]
//     [Tooltip("Zaokrąglaj wejście na pełne wartości (przydatne dla padów).")]
//     public bool ZaokrąglijWejście = true;
//     [Tooltip("Minimalna wartość wejścia w pionie wymagana do akcji (np. wspinanie).")]
//     public float MartwaStrefaPionowa = 0.3f;
//     [Tooltip("Minimalna wartość wejścia w poziomie wymagana do ruchu.")]
//     public float MartwaStrefaPozioma = 0.1f;

//     [Header("Ruch")]
//     [Tooltip("Maksymalna prędkość poruszania się.")]
//     public float MaksymalnaPrędkość = 14f;
//     [Tooltip("Przyspieszenie gracza.")]
//     public float Przyspieszenie = 120f;
//     [Tooltip("Szybkość zatrzymywania się na ziemi.")]
//     public float WyhamowanieNaZiemi = 60f;
//     [Tooltip("Szybkość zatrzymywania się w powietrzu.")]
//     public float WyhamowanieWPowietrzu = 30f;
//     [Tooltip("Stała siła przyciągająca gracza do podłoża (pomocne na pochyłościach).")]
//     public float SiłaPrzyciąganiaDoZiemi = -1.5f;

//     [Header("Skok")]
//     [Tooltip("Siła skoku.")]
//     public float SiłaSkoku = 36f;
//     [Tooltip("Maksymalna prędkość opadania.")]
//     public float MaksymalnaPrędkośćOpadania = 40f;
//     [Tooltip("Przyspieszenie opadania w powietrzu.")]
//     public float PrzyspieszenieOpadania = 110f;
//     [Tooltip("Mnożnik grawitacji przy wczesnym zakończeniu skoku.")]
//     public float ModyfikatorGrawitacjiPrzyWczesnymSkoku = 3f;
//     [Tooltip("Czas na wykonanie skoku po opuszczeniu platformy.")]
//     public float CzasCoyote = 0.15f;
//     [Tooltip("Czas buforowania skoku przed dotknięciem ziemi.")]
//     public float BuforowanieSkoku = 0.2f;

//     private bool czyNaZiemi;
//     private float wejściePoziome;
//     private bool czySkokWciśnięty;
//     private bool czySkokPrzytrzymany;
//     private float licznikBuforaSkoku;
//     private float licznikCzasuCoyote;
//     private float aktualnaPrędkośćPionowa;
//     private float punktApex;

//     void Update()
//     {
//         OdczytajWejście();
//         ObsłużBuforSkoku();
        
//     }

//     void FixedUpdate()
//     {
//         SprawdźCzyNaZiemi();
//         ObsłużRuchPoziomy();
//         ZastosujHamowanie();
//         ObsłużZmianęWysokościSkoku();
//         ZastosujGrawitację();
//         OgraniczPrędkośćOpadania();
//     }

//     void OdczytajWejście()
//     {
//         wejściePoziome = Input.GetAxisRaw("Horizontal");

//         if (ZaokrąglijWejście)
//         {
//             wejściePoziome = Mathf.Round(wejściePoziome);
//         }

//         czySkokWciśnięty = Input.GetButtonDown("Jump");
//         czySkokPrzytrzymany = Input.GetButton("Jump");
//     }

//     void ObsłużRuchPoziomy()
//     {
//         float czynnikKontroli = czyNaZiemi ? 1f : 0.8f;
//         float docelowaPrędkość = wejściePoziome * MaksymalnaPrędkość;
//         float różnicaPrędkości = docelowaPrędkość - body.velocity.x;

//         float szybkośćPrzyspieszenia = Mathf.Abs(docelowaPrędkość) > 0.01f ? Przyspieszenie : (czyNaZiemi ? WyhamowanieNaZiemi : WyhamowanieWPowietrzu);
//         float ruch = Mathf.Clamp(różnicaPrędkości, -szybkośćPrzyspieszenia * Time.fixedDeltaTime, szybkośćPrzyspieszenia * Time.fixedDeltaTime);

//         body.velocity = new Vector2(body.velocity.x + ruch, body.velocity.y);
//         UstawKierunek();
//     }

//     void UstawKierunek()
//     {
//         if (Mathf.Abs(wejściePoziome) > MartwaStrefaPozioma)
//         {
//             float kierunek = Mathf.Sign(wejściePoziome);
//             transform.localScale = new Vector3(kierunek, 1, 1);
//         }
//     }

//     void ObsłużBuforSkoku()
//     {
//         if (czySkokWciśnięty)
//             licznikBuforaSkoku = BuforowanieSkoku;

//         if (czyNaZiemi)
//             licznikCzasuCoyote = CzasCoyote;
//         else
//             licznikCzasuCoyote -= Time.deltaTime;

//         if (licznikBuforaSkoku > 0 && licznikCzasuCoyote > 0)
//         {
//             WykonajSkok();
//             licznikBuforaSkoku = 0;
//         }
//         else
//             licznikBuforaSkoku -= Time.deltaTime;
//     }

//     void WykonajSkok()
//     {
//         aktualnaPrędkośćPionowa = SiłaSkoku;
//         body.velocity = new Vector2(body.velocity.x, aktualnaPrędkośćPionowa);
//     }

//     void ObsłużZmianęWysokościSkoku()
//     {
//         if (!czySkokPrzytrzymany && body.velocity.y > 0)
//         {
//             body.velocity += Vector2.up * Physics2D.gravity.y * (ModyfikatorGrawitacjiPrzyWczesnymSkoku - 1) * Time.deltaTime;
//         }

//         if (!czyNaZiemi)
//         {
//             punktApex = Mathf.InverseLerp(SiłaSkoku, 0, Mathf.Abs(body.velocity.y));
//         }
//     }

//     void ZastosujGrawitację()
//     {
//         float mnożnikGrawitacji = czyNaZiemi ? 1f : (punktApex > 0.5f ? 1f : ModyfikatorGrawitacjiPrzyWczesnymSkoku);
//         aktualnaPrędkośćPionowa -= PrzyspieszenieOpadania * mnożnikGrawitacji * Time.fixedDeltaTime;

//         body.velocity = new Vector2(body.velocity.x, aktualnaPrędkośćPionowa);
//     }

//     void OgraniczPrędkośćOpadania()
//     {
//         if (body.velocity.y < -MaksymalnaPrędkośćOpadania)
//         {
//             body.velocity = new Vector2(body.velocity.x, -MaksymalnaPrędkośćOpadania);
//         }
//     }

//     void SprawdźCzyNaZiemi()
//     {
//         czyNaZiemi = Physics2D.OverlapBox(groundCheck.bounds.center, groundCheck.bounds.size, 0, groundMask);

//         if (czyNaZiemi)
//         {
//             licznikCzasuCoyote = CzasCoyote;
//             aktualnaPrędkośćPionowa = Mathf.Max(aktualnaPrędkośćPionowa, 0);
//         }
//     }

//     void ZastosujHamowanie()
//     {
//         if (czyNaZiemi && Mathf.Abs(wejściePoziome) < MartwaStrefaPozioma)
//         {
//             body.velocity = new Vector2(body.velocity.x * WyhamowanieNaZiemi * Time.fixedDeltaTime, body.velocity.y);
//         }
//     }

//     private void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireCube(groundCheck.bounds.center, groundCheck.bounds.size);
//     }
// }
