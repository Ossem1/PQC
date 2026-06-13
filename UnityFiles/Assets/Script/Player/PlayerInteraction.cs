using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

public class PlayerInteraction : MonoBehaviour
{
    // This script is meant to make it easier to know when the player is interacting with something. Any
    // object will be able to subscribe to the event and know when its happening.

    public static event Action onInteractPressed;
    public static event Action onInteractReleased;
    public static event Action interaction;

    [SerializeField] private InputActionReference interactionAction;

    private void OnEnable()
    {
        interactionAction.action.started += OnInteract;
        interactionAction.action.canceled += OnInteract;

        interactionAction.action.Enable();
    }

    private void OnDisable()
    {
        interactionAction.action.started -= OnInteract;
        interactionAction.action.canceled -= OnInteract;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            onInteractPressed?.Invoke();
        }
        else if (context.canceled)
        {
            onInteractReleased?.Invoke();
        }
    }

    public static void Interact()
    {
        interaction?.Invoke();
    }
}
