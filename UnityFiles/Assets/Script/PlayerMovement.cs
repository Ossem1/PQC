using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private float speed;
    InputAction horizontalInput;
    InputAction verticalInput;
    string currentRealm;  //assigned at start and when switched, used for selecting available actions.

    void Start()
    {
        //Get inputs from unity input system, horizontal input returns a vector, vertical only returns if pressed.
        //actions must be assigned within the input manager.
        horizontalInput = InputSystem.actions.FindAction("HorizontalMovement");
        verticalInput = InputSystem.actions.FindAction("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        VerticalMovement();
    }

    void VerticalMovement()
    {
        Vector2 movementValue = horizontalInput.ReadValue<Vector2>(); //This returns 1 or -1
    }

    void HorizontalMovement()
    {
        
    }
    void OnDisable() //Cleanup for current Ienumators and ongoing processes.
    {
        
    }
}
