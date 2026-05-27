using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    //Values for player movement, can be adjusted in the inspector.
    [SerializeField]private float baseSpeed;
    private float currentSpeed;
    [SerializeField]private float jumpHeight;
    [SerializeField]private float peakHeightTime;
    [SerializeField]private float coyoteTime;
    [SerializeField]private float fallMultiplier;
    private float chargeMultipler = 1f;
    private float groundTimer;
    private bool canJump;
    //Inputs for player movement.
    InputAction horizontalInput;
    InputAction verticalInput;

    //Values needed for movement and actions.
    Rigidbody2D rb;
    string currentRealm;
    private bool isFacingRight = true;
    private SpriteRenderer sprite;

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
        HorizontalMovement();
        //RealityVerticalMovement();
        QuantumVerticalMovement();
    }
    //Gets exact amount of force needed to obtain specific height
    float JumpForce()
    {
        float gravity = Physics2D.gravity.y * rb.gravityScale;
        return Mathf.Sqrt(-2 * gravity * jumpHeight);
    }

    //Reality vertical movement has a charged long jump
    //Player should fall faster once jump input is released, or when falling
    //Charged jumps should freeze player movement, and when releaed push player forward slightly
    void RealityVerticalMovement()
    {
        if(canJump)
        {
            if (verticalInput.IsPressed() && chargeMultipler <= 1.5f)
            {
                chargeMultipler += Time.deltaTime;
                Debug.Log("Jump is being charged");
            } else if (verticalInput.WasReleasedThisFrame())
            {
                rb.AddForce(Vector2.up * (JumpForce() * chargeMultipler),ForceMode2D.Impulse);
                chargeMultipler = 1;
            }
        }
        if (rb.linearVelocity.y <= 2f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier) * Time.fixedDeltaTime;
        }
    }
    //Singular jump
     void QuantumVerticalMovement()
    {
        if(verticalInput.WasPressedThisFrame() && canJump)
        {
            rb.AddForce(Vector2.up * JumpForce(),ForceMode2D.Impulse);
            Debug.Log("jump Activated");
        } 
        if (!verticalInput.IsPressed() || rb.linearVelocity.y <= .2f)
        {
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
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
            Flip();
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
    void OnDisable() //Cleanup for current Ienumators and ongoing processes.
    {
 
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
