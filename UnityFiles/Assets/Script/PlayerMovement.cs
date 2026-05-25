using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    //Values for player movement, can be adjusted in the inspector.
    [SerializeField]private float baseSpeed;
    private float currentSpeed;
    [SerializeField]private float jumpForce;
    [SerializeField]private float peakHeightTime;
    [SerializeField]private float coyoteTime;
    //Inputs for player movement.
    InputAction horizontalInput;
    InputAction verticalInput;

    //Values needed for movement and actions.
    Rigidbody2D rb;
    string currentRealm;  //assigned at start and when switched, used for selecting available actions.
    private bool isFacingRight = true;
    private SpriteRenderer sprite;

    void Start()
    {
        //Get inputs from unity input system, horizontal input returns a vector, vertical only returns if pressed.
        //actions must be assigned within the input manager.
        horizontalInput = InputSystem.actions.FindAction("HorizontalMovement");
        verticalInput = InputSystem.actions.FindAction("Jump");

        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        HorizontalMovement();
    }

    void VerticalMovement()
    {
       


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
        StartCoroutine(StopHorizontalMovement());
    }

    void CoyoteTiming()
    {

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
}
