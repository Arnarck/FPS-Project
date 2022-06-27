using UnityEngine;

public class DebugInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GI.thirst.IncreaseValue(50f);
            GI.hunger.IncreaseValue(50f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GI.player.change_health_amount(50f * GI.hunger.RecoverMultiplier);
        }
    }
}
