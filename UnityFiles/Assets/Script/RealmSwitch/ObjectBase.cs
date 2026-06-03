using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    // This will be the parent object for all objects.

    [SerializeField] protected bool realityRealm = true;         // This is the bool that all objects will look to, to make sure they're in the correct realm.

    [Header("Normal Realm")]
    [SerializeField] private bool isPushable = false;
    [SerializeField] private bool isPressureplate = false;
    [SerializeField] private bool isMoving = false;
    [SerializeField] private Vector2 point1;
    [SerializeField] private Vector2 point2;

    [Space]
    [Header("Quantum Realm")]
    [SerializeField] private bool isPushable_q = false;
    [SerializeField] private bool isPressureplate_q = false;
    [SerializeField] private bool isMoving_q = false;
    [SerializeField] private Vector2 point1_q;
    [SerializeField] private Vector2 point2_q;

    private void Awake()                                     // This subscribes the object to the switching event
    {
        RealmSwitchController.realmSwitched += RealmSwapped;
    }

    private void OnDisable()                                    // This unsubscribes the object, in the event the object is destroyed
    {
        RealmSwitchController.realmSwitched -= RealmSwapped;
    }

    void FixedUpdate()
    {
        if (realityRealm)
        {

        }
        else if (!realityRealm)
        {

        }
    }

    private void RealmSwapped()                                 // This runs when the event happens. Just flip-flips. Can technically be modified, if you make it unprotected.
    {                                                           // But unless it needs to be, don't be silly, wrap your willy
        realityRealm = !realityRealm;
    }
}
