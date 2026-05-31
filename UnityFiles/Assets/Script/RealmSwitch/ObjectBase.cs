using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    // This will be the parent object for all objects.

    private void OnEnable()
    {
        RealmSwitchController.realmSwitchAction += RealmSwapped();
    }

    private void OnDisable()
    {
        RealmSwitchController.realmSwitchAction -= RealmSwapped();
    }

    private void RealmSwapped()
    {
        currentRealm = RealmSwitchController;
    }
}
