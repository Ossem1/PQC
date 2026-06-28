using UnityEngine;

public class RealmCurtain : MonoBehaviour
{
    // This will either hide or reveal the quantum and natural layer
    private Camera cam;

    private bool realityRealm = true;
    void Awake()
    {
        cam = GetComponent<Camera>();
        RealmSwitchController.realmSwitched += RealmSwitch;
    }


    void RealmSwitch()
    {
        if (realityRealm)
        {
            cam.cullingMask |= (1 << LayerMask.NameToLayer("Quantum"));
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Normal"));
            realityRealm = !realityRealm;
            Debug.Log("Cam switching to Quantum");
        }
        else if (!realityRealm)
        {
            cam.cullingMask |= (1 << LayerMask.NameToLayer("Normal"));
            cam.cullingMask &= ~(1 << LayerMask.NameToLayer("Quantum"));
            realityRealm = !realityRealm;
            Debug.Log("Cam switching to Reality");
        }
    }
}
