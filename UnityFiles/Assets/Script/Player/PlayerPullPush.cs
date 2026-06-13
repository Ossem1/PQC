using UnityEngine;

public class PlayerPullPush : MonoBehaviour
{
    // This is what handles the player pulling and pushing objects

    private ObjectBase objectScript;
    private GameObject touchedObject;

    private bool _beingPulled = false;

    private FixedJoint2D joint;

    void Awake()
    {
        joint = GetComponent<FixedJoint2D>();
        PlayerInteraction.onInteractPressed += StartPull;
        PlayerInteraction.onInteractReleased += StopPull;
    }

    void FixedUpdate()
    {
        if (objectScript != null)
        {
            if (objectScript.isPushable && !objectScript.realityRealm || objectScript.isPushable_q && objectScript.realityRealm)
            {
                DetachObject();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.TryGetComponent<ObjectBase>(out objectScript))
        {
            touchedObject = col.gameObject;
            Debug.Log(col.gameObject.name);
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject == touchedObject)
        {
            touchedObject = null;
            Debug.Log(col.gameObject.name);
        }
    }

    void StartPull()
    {
        if (objectScript != null)
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
        objectScript.rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        joint.enabled = true;
    }

    void DetachObject()
    {
        joint.enabled = false;
        objectScript.isPulled = false;
        objectScript.rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        joint.connectedBody = null;
        _beingPulled = false;
    }
        
}
