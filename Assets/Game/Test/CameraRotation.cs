using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public float max_min_x_angle = 45f;
    public float rotation_speed = 5f;
    public Vector3 camera_target_rotation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        camera_target_rotation.x += -Input.GetAxis("Mouse Y") * rotation_speed;
        camera_target_rotation.y += Input.GetAxis("Mouse X") * rotation_speed;

        camera_target_rotation.x = Mathf.Clamp(camera_target_rotation.x, -max_min_x_angle, max_min_x_angle);
        if (camera_target_rotation.y >= 360f) camera_target_rotation.y -= 360f;
        else if (camera_target_rotation.y <= -360f) camera_target_rotation.y += 360f;

        Camera.main.transform.localRotation = Quaternion.Euler(camera_target_rotation);
    }
}
