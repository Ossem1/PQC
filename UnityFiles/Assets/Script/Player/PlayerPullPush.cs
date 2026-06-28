using UnityEngine;

public class PlayerPullPush : MonoBehaviour
{
    // This is what handles the player pulling and pushing objects

    private ObjectBase objectScript;
    private GameObject touchedObject;

    private PlayerMovement movementScript;

    private bool _beingPulled = false;

    private FixedJoint2D joint;

    void Awake()
    {
        joint = GetComponent<FixedJoint2D>();
        PlayerInteraction.onInteractPressed += StartPull;
        PlayerInteraction.onInteractReleased += StopPull;
    }

    void Start()
    {
        movementScript = GetComponentInParent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        if (objectScript != null)
        {
            if (objectScript.isPushable && !objectScript.realityRealm || objectScript.isPushable_q && objectScript.realityRealm || !movementScript.IsGrounded())
            {
                DetachObject();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<ObjectBase>(out objectScript) && movementScript.IsGrounded())
        {
            foreach (ContactPoint2D contact in col.contacts)
            {
                Debug.Log(contact.normal.x + " " + contact.normal.y);
                if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
                {
                    touchedObject = col.gameObject;
                }
                else
                {
                    touchedObject = null;
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject == touchedObject)
        {
            touchedObject = null;
        }
    }

    void StartPull()
    {
        if (objectScript != null && touchedObject != null)
        {
            if (objectScript.isPushable && objectScript.realityRealm || objectScript.isPushable_q && !objectScript.realityRealm)
            {
                AttachObject();
            }   
        }
    }

    void StopPull()
    {
        if (_beingPulled)
        {
            DetachObject();
        }
    }

    void AttachObject()
    {
        _beingPulled = true;
        joint.connectedBody = objectScript.rb;
        objectScript.isPulled = true;
        joint.enabled = true;
        Debug.Log("AttachingObject");
    }

    void DetachObject()
    {
        joint.enabled = false;
        objectScript.isPulled = false;
        joint.connectedBody = null;
        _beingPulled = false;
        Debug.Log("DetachingObject");
    }
        
}
