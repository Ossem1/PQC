using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    // This will be the parent object for all objects.

    [SerializeField] protected bool normalRealm = true;         // This is the bool that all objects will look to, to make sure they're in the correct realm.

    private void OnEnable()                                     // This subscribes the object to the switching event
    {
        RealmSwitchController.realmSwitched += RealmSwapped;
    }

    private void OnDisable()                                    // This unsubscribes the object, in the event the object is destroyed
    {
        RealmSwitchController.realmSwitched -= RealmSwapped;
    }

    private void RealmSwapped()                                 // This runs when the event happens. Just flip-flips. Can technically be modified, if you make it unprotected.
    {                                                           // But unless it needs to be, don't be silly, wrap your willy
        normalRealm = !normalRealm;
    }
}
