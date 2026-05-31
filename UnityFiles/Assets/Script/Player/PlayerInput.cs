using System.Collections;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    //Inputs for player movement.
    InputAction horizontalInput;
    InputAction verticalInput;
    InputAction chargeInput;
    // Realm switch input
    InputAction realmSwitcheroo;
    
    [Header("Horizontal Movment Inputs")]
    public Vector2 movementInput;
    [HideInInspector]public bool movementStarted;
    [HideInInspector]public bool movementStoped;
    [Header("Vertical Movment Inputs")]
    public bool jumpPressed;
    public bool jumpReleased;
    public bool jumpHeld;

    [Header("Action Inputs")]
    [HideInInspector]public bool chargeAction;
    [HideInInspector]public bool realmSwapAction;


    void Start()
    {
        //Get inputs from unity input system, horizontal input returns a vector, vertical only returns if pressed.
        //actions must be assigned within the input manager.
        horizontalInput = InputSystem.actions.FindAction("HorizontalMovement");
        verticalInput = InputSystem.actions.FindAction("Jump");
        chargeInput = InputSystem.actions.FindAction("Charge");
        realmSwitcheroo = InputSystem.actions.FindAction("RealmSwitch");
    }
    void InputReader()
    {
        //Horizontal Inputs
        movementInput = horizontalInput.ReadValue<Vector2>();
        movementStarted |= horizontalInput.WasPressedThisFrame();
        movementStoped |= horizontalInput.WasReleasedThisFrame();

        //Vertical Inputs
        jumpPressed |= verticalInput.WasPressedThisFrame();
        jumpReleased |= verticalInput.WasReleasedThisFrame();
        jumpHeld = verticalInput.IsPressed();

        //Action Inputs
        chargeAction = chargeInput.WasPressedThisFrame();
        realmSwapAction = realmSwitcheroo.WasPressedThisFrame();
    }
    void RealmSwitchInput()
    {
        //Swap Input
        if (realmSwapAction)
        {
            RealmSwitchController.SwapRealm();      //Invokes the swap realm event
        }
    }
    public void ResetInputs()
    {
        movementStarted = false;
        movementStoped = false;
        jumpPressed = false;
        jumpReleased = false;
        chargeAction = false;  
    }

    void Update()
    {
        InputReader();
        RealmSwitchInput();
    }

}
