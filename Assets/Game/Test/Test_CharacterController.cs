using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_CharacterController : MonoBehaviour
{
    [HideInInspector] public Vector3 fp_camera_start_rotation;
    [HideInInspector] public Vector3 camera_rotation;
    [HideInInspector] public Vector3 dp;


    public Test_CrossPlatformInput input;

    [Header("Movement")]
    public float friction = 15f;
    public float acceleration = 100f;

    [Header("Camera")]
    public Transform fp_camera;
    public float mouse_sensitivity = 2f;
    public float camera_min_yall = -60f;
    public float camera_max_yall = 60f;

    // Start is called before the first frame update
    void Start()
    {
        fp_camera_start_rotation = fp_camera.localEulerAngles;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 ddp = Vector3.zero;
        float fdt = Time.fixedDeltaTime;

        { // Character Movement
            ddp += input.movement_direction();
        }

        { // Gravity
            //ddp.y -= -.3f;
        }

        { // Apply movement
            ddp *= acceleration;
            ddp -= dp * friction;


            /*
             a = acceleration
             v = final velocity
             u = initial velocity
             t = time taken
             s = displacement
             */

            // https://spm-physics-402.blogspot.com/2009/01/acceleration.html

            // v = u + at
            // dp = dp + ddp * fdt
            // "dp" is both "initial velocity (u)" and "final velocity (v)"
            dp += ddp * fdt;
            
            // s = ut + 1/2at^2
            // s = (u * t) + (1/2 * a * t^2)
            // transform.position = transform.position + s ("s" is the result of the formula above. The displacement)
            transform.position += (dp * fdt) + (ddp * fdt * fdt * 0.5f);
        }
    }

    private void Update()
    {
        { // Camera Movement
            camera_rotation += input.camera_look_direction() * mouse_sensitivity;
            Debug.Log(input.camera_look_direction());

            // Clamp rotation
            camera_rotation.x = Mathf.Clamp(camera_rotation.x, camera_min_yall, camera_max_yall);
            if (camera_rotation.y < 0f) camera_rotation.y += 360f;
            else if (camera_rotation.y >= 360f) camera_rotation.y -= 360f;

            fp_camera.localRotation = Quaternion.Euler(fp_camera_start_rotation + camera_rotation);
        }
    }
}


//ddp *= acceleration;
//ddp -= dp * friction;


///*
// a = acceleration
// v = final velocity
// u = initial velocity
// t = time taken
// s = displacement
// */

//// https://spm-physics-402.blogspot.com/2009/01/acceleration.html

//// v = u + at
//// dp = dp + ddp * fdt
//// "dp" is both "initial velocity (u)" and "final velocity (v)"
//dp += ddp * fdt;

//// s = ut + 1/2at^2
//// s = (u * t) + (1/2 * a * t^2)
//// transform.position = transform.position + s ("s" is the result of the formula above. The displacement)
//transform.position += (dp * fdt) + (ddp * fdt * fdt * 0.5f);
