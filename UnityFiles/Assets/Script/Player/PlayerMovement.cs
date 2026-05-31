using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

    //Ensures required components are present for script to function
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(PlayerInput))]

    ///Variables
    /// Base Speed :      Basic speed at which player moves
    /// acceleration:     Time: Time it takes to get to max speed
    /// deceleration:     Time: Time to get to a stand still, or stop movement
    /// Jump Height:      Target Jump height, this is the peak of a jump/vertex
    /// Peak cutoff:      Distance away from vertex to start falling, the higher the value the snappier the fall/rise
    /// Coyote Time:      Time allowed for the player to jump after not being grounded
    /// Fall Multiplier:  When falling, this value is a increase to gravity/fall speed
    /// Max Jump Charge:  Increase in distance when using charged jump, only additive to total height.


public class PlayerMovement : ObjectBase
{
    //Values for player movement, can be adjusted in the inspector.
    [Header("Horizontal Movement")]
    [SerializeField]private float _baseSpeed;
    private float _currentSpeed;
    
    [SerializeField] [Range(0f,1f)] private float _accelerationTime;
    [SerializeField] [Range(0f,1f)] private float _decelerationTime;
   
    //Variables for jump mechanics
    [Header("Jump Variables")]
    [SerializeField]private float _realityJumpHeight;
    [SerializeField]private float _quantumJumpHeight;
    [SerializeField] [Range(0f,3f)]private float _peakCutoff;
    [SerializeField] [Range(0f,.5f)] private float _coyoteTime;
    [SerializeField] [Range(.5f,2f)]private float _fallMultiplier;
    [SerializeField] [Range(0f,4f)] private float _maxJumpCharge;
    private float _chargeMultipler = 1f;
    private bool _toggleCharge;
    private float _groundTimer;
    private bool _canJump;

    //Inputs for player movement.
    PlayerInput playerInput;

    //Values needed for movement and actions.
    Rigidbody2D rb;
    private bool updatedRealm;  //Current Assigned Realm
    private bool _isFacingRight = true;
    private SpriteRenderer sprite;

    [Header("Ground Check Requirments")]
    //Variables used IsGrounded function
    [SerializeField]private float _groundCastDistance;
    [SerializeField]private Vector2 _groundCastSize;
    [SerializeField]private LayerMask groundLayer;

    //Coroutines
    Coroutine _stopMovement;
    Coroutine _changeSpeed;

    //Delegated Methods
    private delegate void VerticalMovementBehavior();
    private VerticalMovementBehavior currentVerticalMovement; 
    private delegate void currentChargeAction();
    private VerticalMovementBehavior chargeAction;

    //Gets all components needed
    void GetComponents()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
    }
    void OnEnable()
    {
        GetComponents();           
        updatedRealm = !realityRealm;
        RealmSwitchController.realmSwitched += AssignMovementBehavior;      //when realm is switched this will activate changing movement type
        AssignMovementBehavior();
    }
    // Update is called once per frame
    void Update()
    {
        if(chargeAction != null) { chargeAction(); }      //Current Assigned action to charge key
        CoyoteTiming();
        Flip();
    }

    void FixedUpdate()
    {
        if (!_toggleCharge)
        {
            HorizontalMovement();
        }
        if(currentVerticalMovement != null) {currentVerticalMovement();} else { RealityVerticalMovement();}                     //Invokes the current movement type

        playerInput.ResetInputs();
    }
    //This get used on Realm swap to change current behaviors of movement
    //Cancels all chargeAction, and correctly adjust for the new movement
    void AssignMovementBehavior()
    {
        if (updatedRealm != realityRealm)              //Assigns correct vertical movement to event
        {
            if (realityRealm)
            {
                chargeAction = ToggleCharge;
                currentVerticalMovement = RealityVerticalMovement;
                Debug.Log("Current Realm = Normal");
                updatedRealm = realityRealm;
            }
            else
            {
                chargeAction = null;
                _toggleCharge = false;
                currentVerticalMovement = QuantumVerticalMovement;
                Debug.Log("Current Realm = Quantum");
                updatedRealm = realityRealm;
            }
        }
    }

    //Gets exact amount of force needed to obtain specific height
    float JumpForce(float jumpHeight)
    {
        float gravity = Physics2D.gravity.y * rb.gravityScale;
        return Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    //Reality vertical movement has a charged long jump
    //Toggle charge and hold jump action to use charged jump
    //freeze movement while a charge jump is active
    void RealityVerticalMovement()
    {
        //Charge Jump Mechanic
        Vector2 chargedJumpDirection = new Vector2(GetDirection()/2f,1).normalized; //Get diagonal upward direction
        if(_toggleCharge == true)
        {
            if (playerInput.jumpHeld && _chargeMultipler <= _maxJumpCharge && _canJump)
            {
                _chargeMultipler += Time.fixedDeltaTime;
            } 
            
            else if (playerInput.jumpReleased)
            {
                rb.AddForce(chargedJumpDirection * (JumpForce(_realityJumpHeight) + _chargeMultipler),ForceMode2D.Impulse);
                _chargeMultipler = 1;
            }
            if (rb.linearVelocity.y <= _peakCutoff && !IsGrounded())
            {
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * _fallMultiplier) * Time.fixedDeltaTime;
            }
        } else
        {  
            //Regular Jump
            if(playerInput.jumpPressed && _canJump && !playerInput.chargeAction)
            {
                rb.AddForce(Vector2.up * JumpForce(_realityJumpHeight),ForceMode2D.Impulse);
            } 
            if ((!playerInput.jumpHeld || rb.linearVelocity.y <= _peakCutoff) && !IsGrounded())
            {
                rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * _fallMultiplier) * Time.fixedDeltaTime;

            }
        }
    }

    //Singular jump
    void QuantumVerticalMovement()
    {
        if(playerInput.jumpPressed && _canJump)
        {
            rb.AddForce(Vector2.up * JumpForce(_quantumJumpHeight),ForceMode2D.Impulse);
        } 
        if ((!playerInput.jumpHeld || rb.linearVelocity.y <= _peakCutoff) && !IsGrounded())
        {
            rb.linearVelocity += Vector2.up * (Physics2D.gravity.y * _fallMultiplier) * Time.fixedDeltaTime;
        }
    }

    //Controls only horizontal movement.
    //This includes a slowdown, and input translation to velocity.
    //builds up speed, and slows down gradually when input is released.
    void HorizontalMovement()
    {
        if (playerInput.movementStarted)
        {
            if(_stopMovement != null)
            {   
                StopCoroutine(_stopMovement);
                _stopMovement = null;
            }
            if(_changeSpeed != null)
            {
                StopCoroutine(_changeSpeed);
            }
            _changeSpeed = StartCoroutine(Accelerate());
        }
        if (playerInput.movementInput != Vector2.zero)
        {
            rb.linearVelocity = new Vector2(playerInput.movementInput.x * _currentSpeed, rb.linearVelocity.y);
        }
        else if(playerInput.movementStoped)
        {
            if(_changeSpeed != null)
            {   
                StopCoroutine(_changeSpeed);
                _changeSpeed = null;
            }
            if(_stopMovement != null)
            {
                StopCoroutine(_stopMovement);
            }
            _stopMovement = StartCoroutine(Decelerate());
        }
    }

    //Dynamically changes speed of player, requires a time for duration and target speed
    //Only interacts with the speed variable
    IEnumerator Accelerate() 
    {
        float elapsed = 0f;
        float startSpeed = _currentSpeed;
        while (elapsed < _accelerationTime)
        {
            // Bail early if input was released mid-acceleration
            if (playerInput.movementStoped)
            {
                yield break;
            }
            _currentSpeed = Mathf.Lerp(startSpeed, _baseSpeed, elapsed / _accelerationTime);
            elapsed += Time.deltaTime;
            yield return null;
        }
        _currentSpeed = _baseSpeed;
    }

    //Stops horizontal movement gradually
    //This will take over horizontal velocity
    IEnumerator Decelerate()
    {
         _currentSpeed = 0f;
        float elapsed = 0f;
        float startVelocityX = rb.linearVelocity.x;
        while (elapsed < _decelerationTime)
        {
            float t = elapsed / _decelerationTime;
            rb.linearVelocity = new Vector2(Mathf.Lerp(startVelocityX, 0f, t), rb.linearVelocity.y);
            elapsed += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    //actions that occur when player lands
    void OnLand()
    {
        _toggleCharge = false;
        _chargeMultipler = 1f;
        //Play landing particles
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            OnLand();
        }
    }
    //allows player moment to jump when not grounded
    void CoyoteTiming()
    {
        //Sets conditions for coyote time
        if (!IsGrounded())
        {
            _groundTimer -= Time.deltaTime;
        }
        else
        {
            _groundTimer = _coyoteTime;
        }
        _canJump = _groundTimer >= 0f;
    }

    //Using charge will freeze movement, but allow for specific actions
    void ToggleCharge()
    {
        if (playerInput.chargeAction && !playerInput.jumpHeld && IsGrounded())
        {
            _toggleCharge = !_toggleCharge;
            if(_toggleCharge)
            {
                if(_stopMovement != null)
                {
                    StopCoroutine(_stopMovement);
                }
                _stopMovement = StartCoroutine(Decelerate());
            }
        }
    }

    //Flips sprite based on movement direction.
    private void Flip()
    {
        if (_isFacingRight && playerInput.movementInput.x < 0f || !_isFacingRight && playerInput.movementInput.x > 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = sprite.transform.localScale;
            localScale.x *= -1f;
            sprite.transform.localScale = localScale;
        }
    }

    private float GetDirection()
    {
        float direction = (_isFacingRight) ? 1f: -1;
        return direction;
    }

    //Used to find if player is grounded
    public bool IsGrounded()
    {
        return Physics2D.BoxCast(transform.position,_groundCastSize,0,-transform.up, _groundCastDistance, groundLayer);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * _groundCastDistance,_groundCastSize);
    }
}
