using UnityEngine;

[CreateAssetMenu(fileName = "RealmData", menuName = "Scriptable Objects/RealmData")]
public class RealmData : ScriptableObject
{
    public static RealmData instance { get; private set;};
    

    [SerializeField] private RealmSwitchController controller;

    private void OnEnable()
    {
        instance = this;
    }

    public bool currentRealm()
    {
        return controller.currentRealm;
    }
}
