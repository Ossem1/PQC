using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

// This was meant to do much more, but slowly got simplified.
// All this does is house the event, and has a method to trigger it. The input script triggers this.

public class RealmSwitchController : MonoBehaviour
{
    public static event Action realmSwitched;        // This is the event that pings all objects to update

    public static void SwapRealm()                   // This is where the event is triggered
    {
        realmSwitched?.Invoke();
    }
}
