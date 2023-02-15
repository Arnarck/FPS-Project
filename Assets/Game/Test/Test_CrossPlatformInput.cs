using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CrossPlatformInput : MonoBehaviour
{
    public KeyCode move_forward_key;
    public KeyCode move_backward_key;
    public KeyCode move_left_key;
    public KeyCode move_right_key;

    public Vector3 movement_direction()
    {
        Vector3 direction = Vector3.zero;

        if (Input.GetKey(move_forward_key)) direction.z += 1f;
        if (Input.GetKey(move_backward_key)) direction.z -= 1f;
        if (Input.GetKey(move_left_key)) direction.x -= 1f;
        if (Input.GetKey(move_right_key)) direction.x += 1f;

        return direction.normalized;
    }

    public bool moving()
    {
        return movement_direction().magnitude > 0f;
    }

    public Vector3 camera_look_direction()
    {
        Vector3 direction = Vector3.zero;

        float pitch = Input.GetAxis("Mouse Y"); // Y axis displacement rotates around the X axis
        float yall = Input.GetAxis("Mouse X"); // X axis displacement rotates around the Y axis

        direction.x = -pitch;
        direction.y = yall;

        return direction;
    }

    public bool rotating_camera()
    {
        return camera_look_direction().magnitude > 0f;
    }
}
