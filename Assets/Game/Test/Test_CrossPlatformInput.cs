using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CrossPlatformInput : MonoBehaviour
{
    public KeyCode move_forward_key;
    public KeyCode move_backward_key;
    public KeyCode move_left_key;
    public KeyCode move_right_key;

    public Vector2 movement_direction()
    {
        Vector2 direction = Vector2.zero;

        if (Input.GetKey(move_forward_key)) direction.y += 1f;
        if (Input.GetKey(move_backward_key)) direction.y -= 1f;
        if (Input.GetKey(move_left_key)) direction.x -= 1f;
        if (Input.GetKey(move_right_key)) direction.x += 1f;

        return direction.normalized;
    }

    public bool moving()
    {
        return movement_direction().magnitude > 0f;
    }

    public Vector3 mouse_direction()
    {
        Vector3 direction = Vector3.zero;
        
        // The longer the mouse moves in a single frame, the higher the value that GetAxis will return
        float pitch = Input.GetAxis("Mouse Y"); // Y axis displacement rotates around the X axis
        float yall = Input.GetAxis("Mouse X"); // X axis displacement rotates around the Y axis

        direction.x = -pitch;
        direction.y = yall;

        return direction;
    }

    public bool rotating_camera()
    {
        return mouse_direction().magnitude > 0f;
    }
}
