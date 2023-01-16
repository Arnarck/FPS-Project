using UnityEngine;

public enum Quadrant
{
    NONE,
    FIRST,
    SECOND,
    THIRD,
    FOURTH,
}

public class BobAnimation : MonoBehaviour
{
    protected float sin_time, cos_time, a_sin_time; // Stores the normalized angle of the wave (0 -> 0; 0.25f -> PI/2; 0.5f -> PI; 0.75f -> 3PI/2; 1f -> 2PI)
    protected float x_this_frame, y_this_frame, a_this_frame; // Stores the parameters to pass to Sin(x) and Cos(y) functions every frame
    protected float sin_this_frame, cos_this_frame, a_sin_this_frame; // Stores the result of Sin(x) and Cos(y) operations every frame.
    protected float current_sin_amplitude, current_cos_amplitude, current_a_sin_amplitude;

    [HideInInspector] public Vector3 displacement, angular_displacement; // The total displacement at a given angle of the cicle.

    [Header("Bob Settings")]
    public MovementState bob_type = MovementState.IDLE;

    [Header("Sin(x)")]
    public float sin_amplitude = 1f; // The "size" of the wave.
    public float sin_frequency = 1f; // The "speed" of the wave.

    [Header("Cos(y)")]
    public float walking_cos_amplitude = 1;
    public float cos_amplitude = 1f;
    public float cos_frequency = 1f;

    [Header("Sin(angular)")]
    public float angular_sin_frequency = 1f; // Sugestion: Different frequences for each axis.
    public Vector3 angular_sin_amplitude;

    // Update is called once per frame
    public void update(float dt, MovementState movement_state)
    {
        {
            if (movement_state == MovementState.IDLE) current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, cos_amplitude, Time.deltaTime * cos_frequency);
            else if (movement_state == MovementState.WALKING) current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, walking_cos_amplitude, Time.deltaTime * cos_frequency);
        }

        { // Updates the animation
            //sin_time = increase_wave_time(sin_time, current_sin_frequency, dt);
            //cos_time = increase_wave_time(cos_time, current_cos_frequency, dt);
            //a_sin_time = increase_wave_time(a_sin_time, current_a_sin_frequency, dt);

            sin_time = increase_wave_time(sin_time, sin_frequency, dt);
            cos_time = increase_wave_time(cos_time, cos_frequency, dt);
            a_sin_time = increase_wave_time(a_sin_time, angular_sin_frequency, dt);
        }

        { // Calculates the sin / cos
            x_this_frame = GI.player.tau * sin_time + 0f;
            y_this_frame = GI.player.tau * cos_time + 0f;
            a_this_frame = GI.player.tau * a_sin_time + 0f;

            sin_this_frame = Mathf.Sin(x_this_frame);
            cos_this_frame = Mathf.Cos(2f * y_this_frame);
            a_sin_this_frame = Mathf.Sin(a_this_frame);
        }

        { // Sets the displacement of the position and rotation base on the sin / cos.
            displacement.x = sin_amplitude * sin_this_frame;
            displacement.y = current_cos_amplitude * cos_this_frame - current_cos_amplitude; // amplitude is subtracting the Y to make cos(0) == 0 instead of cos(0) == 1

            angular_displacement.x = angular_sin_amplitude.x * a_sin_this_frame;
            angular_displacement.y = angular_sin_amplitude.y * a_sin_this_frame;
            angular_displacement.z = angular_sin_amplitude.z * a_sin_this_frame;
        }
    }


    public float increase_wave_time(float wave_time, float wave_frequency, float dt)
    {
        wave_time += dt * wave_frequency;
        if (wave_time >= 1f) wave_time -= 1f; // On "time == 1f", the wave completes the 360° cicle. The value is being reseted to don't trepass the float number limit.

        return wave_time;
    }
}
