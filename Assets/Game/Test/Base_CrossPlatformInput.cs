using UnityEngine;



public enum EInput_State
{
    NONE,

    PRESSED,
    HOLDING,
    RELEASED,

    COUNT,
}

public enum EInput_Action
{
    NONE,

    MOVE_FORWARD,
    MOVE_BACKWARDS,
    MOVE_LEFT,
    MOVE_RIGHT,
    RUN,

    COUNT,
}

[System.Serializable]
public struct FInput_Action
{
    public EInput_Action action;
    public KeyCode key;
}



public class Base_CrossPlatformInput : MonoBehaviour
{
    public FInput_Action[] inputs;

    public bool get_key(EInput_State state, EInput_Action action)
    {
        if (state == EInput_State.NONE || state == EInput_State.COUNT) { Debug.LogError($"{state} is not a valid input state!"); return false; }

        foreach (FInput_Action input in inputs)
        {
            if (input.action == action)
            {
                switch (state)
                {
                    case EInput_State.PRESSED: { if (Input.GetKeyDown(input.key)) return true; else return false; } // KeyDown
                    case EInput_State.HOLDING: { if (Input.GetKey(input.key)) return true; else return false; } // Key
                    case EInput_State.RELEASED: { if (Input.GetKeyUp(input.key)) return true; else return false; } // KeyUp
                }
            }
        }

        return false;
    }

    public Vector3 camera_look_direction()
    {
        Vector3 direction = Vector3.zero;
        direction.x = Input.GetAxis("Mouse X"); // Y axis displacement rotates around the X axis
        direction.y = Input.GetAxis("Mouse Y"); // X axis displacement rotates around the Y axis
        return direction;
    }
}
