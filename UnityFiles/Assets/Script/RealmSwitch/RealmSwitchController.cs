using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System;

public class RealmSwitchController : MonoBehaviour
{
    // This script, despite being the "controller", is incredibly simple. You press button, variable updates.
    // It also triggers an event, which pings all objects to check the realm variable on this script.

    InputAction realmSwitch
    public static event Action realmSwitched;        // This is the event that pings all objects to update
    private bool normalRealm = true;

    private void Start()
    {
        realmSwitch = InputSystem.action.FindAction("RealmSwitch");
    }

    private void RealmSwap(InputAction.CallbackContext context)
    {
        realmSwitched?.Invoke();
    }

    private void SwapRealmBool()
    {
        normalRealm = !normalRealm;
    }

    private void OnEnable()
    {
        realmSwitchAction.action.performed += RealmSwap;
        realmSwitchAction.action.performed += SwapRealmBool;
    }

    private void OnDisable()
    {
        realmSwitchAction.action.performed -= RealmSwap;
        realmSwitchAction.action.performed -= SwapRealmBool;
    }

    void Update()
    {
        Debug.Log(normalRealm);
    }

}
