using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

public class PlayerInteraction : MonoBehaviour
{
    // This script is meant to make it easier to know when the player is interacting with something. Any
    // object will be able to subscribe to the event and know when its happening.

    public static event Action interaction;

    public static void Interact()
    {
        interaction?.Invoke();
    }
}
