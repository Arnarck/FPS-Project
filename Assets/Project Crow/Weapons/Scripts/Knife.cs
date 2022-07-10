using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    float evading_t;
    float last_evade_t;

    public Vector3 desired_move;
    public Vector2 evade_direction;
    public float time_to_evade;
    public float evade_time = 1f;
    public float evade_force = 50f;
    public bool is_evading, can_evade;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Vertical") < 0f || Input.GetAxisRaw("Horizontal") != 0f)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && !is_evading && can_evade)
            {
                evade_direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
                desired_move = GI.fp_camera.transform.forward * evade_direction.y + GI.fp_camera.transform.right * evade_direction.x;
                is_evading = true;
                can_evade = false;
                evading_t = 0f;
                last_evade_t = 0f;
            }
        }

        { // Evading time countdown
            if (is_evading)
            {
                if (evading_t >= evade_time) is_evading = false;
                else evading_t += Time.deltaTime;
            }
        }

        { // Evade cooldown countdown
            if (!is_evading && !can_evade)
            {
                if (last_evade_t >= time_to_evade) can_evade = true;
                else last_evade_t += Time.deltaTime;
            }
        }
    }
}
