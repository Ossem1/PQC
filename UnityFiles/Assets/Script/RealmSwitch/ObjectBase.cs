using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    // This will be the parent object for all objects.

    // Generic variables
    private Vector2 _position;
    private bool _direction = true;  //Variable used for isMoving, true = right, false = left
    protected Rigidbody2D rb;

    [SerializeField] protected bool realityRealm = true;         // This is the bool that all objects will look to, to make sure they're in the correct realm.

    [Header("Reality Realm")]           // All toggleable options for reality, plus the points and movingSpeed settings for isMoving
    [SerializeField] private bool isPushable = false;
    [SerializeField] private bool isPressureplate = false;
    [SerializeField] private bool isMoving = false;
    [Tooltip("The two points the object will go between. X < Y!")]      // Hover over "points"
    [SerializeField] private Vector2 points;
    [SerializeField] private float movingSpeed;

    [Space]
    [Header("Quantum Realm")]           // These are all the toggleable options for quantum
    [SerializeField] private bool isPushable_q = false;
    [SerializeField] private bool isPressureplate_q = false;
    [SerializeField] private bool isMoving_q = false;


    protected virtual void OnEnable()                                     // This subscribes the object to the switching event
    {
        RealmSwitchController.realmSwitched += RealmSwapped;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        _position = transform.position;             // This is set here so it isn't null
    }

    protected virtual void OnDisable()                                    // This unsubscribes the object, in the event the object is destroyed
    {
        RealmSwitchController.realmSwitched -= RealmSwapped;
    }

    protected virtual void FixedUpdate()    // Can institute any reality vs quantum logic here
    {
        if (realityRealm)
        {
            if (isMoving)
            {
                IsMoving();
            }
        }
        else if (!realityRealm)
        {
            if (isMoving_q)
            {
                IsMoving();
            }
        }
    }

    private void RealmSwapped()                                 // This runs when the event happens. Just flip-flips. Can technically be modified, if you make it unprotected.
    {                                                           // But unless it needs to be, don't be silly, wrap your willy
        realityRealm = !realityRealm;
    }

    void IsMoving()                     // This is the method that actually moves the object back and forth.
    {
        _position = rb.position;

        float targetx = _direction ? points.y : points.x;       // If _direction = true, x = y, if false x = x
        Vector2 target = new Vector2(targetx, rb.position.y);   // Target = the above targetx, and default y position

        Vector2 targetDirection = (target - rb.position).normalized;        // Just figures out which way its going

        rb.MovePosition(rb.position + targetDirection * movingSpeed * Time.fixedDeltaTime);         // Moves the object

        if (_direction && rb.position.x >= points.y)        // Direction: true = right, false = left
            _direction = false;                             // If going right but at or past y, go left
        else if (!_direction && rb.position.x <= points.x)  // If going left but at or pasy x, go right
            _direction = true;
    }

    void OnDrawGizmosSelected()             // This draws circles on the location of the points! In fancy technicolor!
    {
        if (isMoving || isMoving_q)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(new Vector3(points.x, transform.position.y, 0), 0.5f);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(new Vector3(points.y, transform.position.y, 0), 0.5f);
        }
    }
}
