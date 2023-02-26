using UnityEngine;

public class Test_Movement : MonoBehaviour
{
    Vector3 dp; // Velocity (Derivative of the Position)

    public float friction = 15f;
    public float acceleration = 100f;

    void FixedUpdate() // Must use FixedUpdate so the "dp" stop moving after some time; On Update, it will keep moving by tiny amounts and rarely will stop
    {
        float fdt = Time.fixedDeltaTime;
        Vector3 input = Vector3.zero;
        Vector3 ddp = Vector3.zero; // Current acceleration (Derivative of the Derivative of the Position)

        if (Input.GetKey(KeyCode.W)) input.z += 1;
        if (Input.GetKey(KeyCode.S)) input.z -= 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;

        ddp += input;
        //ddp.y -= -.3f; // Gravity

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
        // s = (u * t) + (0.5f * a * t * t)
        // transform.position = transform.position + s ("s" is the result of the formula above. The displacement)
        transform.position += (dp * fdt) + (ddp * fdt * fdt * 0.5f);
    }
}
