using UnityEngine;

public class TestObjectSwitch : ObjectBase
{
    // This script is garbage and can be deleted when not needed
    // Legit just a test. On boot it makes sure updated is the opposite of normalRealm from objectbase, so it can make sure to enable or disable the sprite objects properly
    // Then, it does that
    [Space]
    public GameObject quantum;
    public GameObject reality;
    private bool updated;


    protected override void OnEnable()
    {
        base.OnEnable();
        updated = !realityRealm;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (updated != realityRealm)
        {
            if (realityRealm)
            {
                quantum.SetActive(false);
                reality.SetActive(true);
                updated = realityRealm;
            }
            else if (!realityRealm)
            {
                quantum.SetActive(true);
                reality.SetActive(false);
                updated = realityRealm;
            }
        }
    }
}
