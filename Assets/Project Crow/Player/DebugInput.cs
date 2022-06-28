using UnityEngine;

public class DebugInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GI.thirst.increase_value(50f);
            GI.hunger.increase_value(50f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GI.player.change_health_amount(GC.HEALTH_RESTORED_BY_MEDKIT_ITEMS * GI.hunger.restoration_effectiveness);
        }
    }
}
