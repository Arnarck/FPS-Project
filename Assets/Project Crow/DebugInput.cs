using UnityEngine;

public class DebugInput : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            GI.Instance.thirst.IncreaseValue(50f);
            GI.Instance.hunger.IncreaseValue(50f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            GI.Instance.player.ModifyHealthAmount(50f * GI.Instance.hunger.RecoverMultiplier);
        }
    }
}
