using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

    //Ensures required components are present for script to function
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]

public class PlayerMovement : MonoBehaviour
{
    //Values for player movement, can be adjusted in the inspector.
    [Header("Horizontal Movement")]
    [SerializeField]private float baseSpeed;
    private float currentSpeed;

    //Variables for jump mechanics
    [Header("Jump Variables")]
    [SerializeField]private float realityJumpHeight;
    [SerializeField]private float quantumJumpHeight;
    [SerializeField] [Range(0f,.5f)] private float coyoteTime;
    [SerializeField] [Range(.5f,2f)]private float fallMultiplier;
    [SerializeField] [Range(0f,4f)] private float maxJumpCharge;
    private float chargeMultipler = 1f;
    private bool toggleCharge;
    private float groundTimer;
    private bool canJump;


    //Inputs for player movement.
    InputAction horizontalInput;
    InputAction verticalInput;
    InputAction charge;

    //Values needed for movement and actions.
    Rigidbody2D rb;
    string currentRealm;
    private bool isFacingRight = true;
    private SpriteRenderer sprite;

    [Header("Ground Check Requirments")]
    //Variables used IsGrounded function
    [SerializeField]private float groundCastDistance;
    [SerializeField]private Vector2 groundCastSize;
    [SerializeField]private LayerMask groundLayer;

    void Start()
    {
        //Get inputs from unity input system, horizontal input returns a vector, vertical only returns if pressed.
        //actions must be assigned within the input manager.
        horizontalInput = InputSystem.actions.FindAction("HorizontalMovement");
        verticalInput = InputSystem.actions.FindAction("Jump");
        charge = InputSystem.actions.FindAction("Charge");

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

    }

    void FixedUpdate()
    {
        CoyoteTiming();
    }
    // Update is called once per frame
    void Update()
    {
        if (!toggleCharge)
        {
            HorizontalMovement();
        }
        RealityVerticalMovement();

        Flip();
        //QuantumVerticalMovement();
    }
    //Gets exact amount of force needed to obtain specific height
    float JumpForce(float jumpHeight)
    {
        float gravity = Physics2D.gravity.y * rb.gravityScale;
        return Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    //Reality vertical movement has a charged long jump
    //
    //
    void RealityVerticalMovement()
    {
        ToggleCharge();
        //Charge Jump Mechanic
        Vector2 chargedJumpDirection = new Vector2(GetDirection()/2,1); //Get diagonal upward direction
        if(toggleCharge == true)
        {
            if (verticalInput.IsPressed() && chargeMultipler <= maxJumpCharge && canJump)
            {
                chargeMultipler += Time.deltaTime;
                Debug.Log("Jump is being charged");
            } else if (verticalInput.WasReleasedThisFrame())
            {
                rb.AddForce(chargedJumpDirection * (JumpForce(realityJumpHeight) + chargeMultipler),ForceMode2D.Impulse);
                chargeMultipler = 1;
                Debug.Log("Charged Jump");
            }
            if (rb.linearVelocity.y <= 6f && !IsGrounded())
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.fixedDeltaTime;
            }
        } else
        {  
            //Regular Jump
            if(verticalInput.WasPressedThisFrame() && canJump && !charge.IsPressed())
            {
                rb.AddForce(Vector2.up * JumpForce(quantumJumpHeight),ForceMode2D.Impulse);
                Debug.Log("regular jump");
            } 
            if ((!verticalInput.IsPressed() || rb.linearVelocity.y <= 6f) && !IsGrounded())
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.fixedDeltaTime;
                Debug.Log("Falling");
            }
        }
    }

    //Singular jump
     void QuantumVerticalMovement()
    {
        if(verticalInput.WasPressedThisFrame() && canJump)
        {
            rb.AddForce(Vector2.up * JumpForce(quantumJumpHeight),ForceMode2D.Impulse);
            Debug.Log("jump Activated");
        } 
        if ((!verticalInput.IsPressed() || rb.linearVelocity.y <= 6f) && !IsGrounded())
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.fixedDeltaTime;
            Debug.Log("Falling");
        }
    }
    //Controls only horizontal movement.
    //This includes a slowdown, and input translation to velocity.
    //builds up speed, and slows down gradually when input is released.
    void HorizontalMovement()
    {
        Vector2 movementValue = horizontalInput.ReadValue<Vector2>(); //This returns 1 or -1 on X axis
        if (currentSpeed != baseSpeed && horizontalInput.WasPressedThisFrame())
        {
            StartCoroutine(ChangeSpeed(baseSpeed, .25f));
        }
        if (horizontalInput.IsInProgress())
        {
            rb.linearVelocity = new Vector2(movementValue.x * currentSpeed, rb.linearVelocity.y);
        }
        else if(horizontalInput.WasReleasedThisFrame())
        {
            StartCoroutine(StopHorizontalMovement());
        }
    }
    //Dynamically changes speed of player, requires a time for duration and target speed
    //Only interacts with the speed variable
    IEnumerator ChangeSpeed(float targetSpeed, float duration) 
    {
        float timeElapsed = 0f;
        float initialSpeed = currentSpeed;
        while (timeElapsed < duration)
        {
            currentSpeed = Mathf.Lerp(initialSpeed, targetSpeed, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            if (horizontalInput.WasReleasedThisFrame())
            {
                currentSpeed = 0f;
                yield break;
            }
            yield return null;
        }
        currentSpeed = targetSpeed; 
    }
    //Stops horizontal movement gradually
    //This will take over horizontal velocity
    IEnumerator StopHorizontalMovement()
    {
        currentSpeed = 0f;
        while (rb.linearVelocity.x > 0.1f || rb.linearVelocity.x < -0.1f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x * 0.9f, rb.linearVelocity.y);
            yield return new WaitForSeconds(.02f);
        }
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    //actions that occur when player lands
    void landing()
    {
        toggleCharge = false;
        //Play landing particles
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            landing();
        }
    }
    //allows player moment to jump when not grounded
    void CoyoteTiming()
    {
        //Sets conditions for coyote time
        if (!IsGrounded())
        {
            groundTimer -= Time.deltaTime;
        }
        else
        {
            groundTimer = coyoteTime;
        }
        //Sets if player can jump
        if(groundTimer >= 0f)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }
    }

        //Using charge will freeze movement, but allow for specific actions
    void ToggleCharge()
    {
        if (charge.WasPressedThisFrame() && !verticalInput.IsPressed() && IsGrounded())
        {
            toggleCharge = !toggleCharge;
            if(toggleCharge == false)
            {
                StartCoroutine(StopHorizontalMovement());
            }
        }
    }

    //Flips sprite based on movement direction.
    private void Flip()
    {
        if (isFacingRight && horizontalInput.ReadValue<Vector2>().x < 0f || !isFacingRight && horizontalInput.ReadValue<Vector2>().x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = sprite.transform.localScale;
            localScale.x *= -1f;
            sprite.transform.localScale = localScale;
        }
    }

    private float GetDirection()
    {
        if (isFacingRight)
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }

    //Used to find if player is grounded
    public bool IsGrounded()
    {
        if(Physics2D.BoxCast(transform.position,groundCastSize,0,-transform.up, groundCastDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position-transform.up * groundCastDistance,groundCastSize);
    }
}
