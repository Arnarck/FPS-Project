using UnityEngine;



[System.Serializable]
public struct FBobAnimationSettings
{
    [Header("Directional")]
    public float amplitude_update_rate;
    public float frequency_update_rate;
    public Vector3 amplitude;
    public Vector3 frequency;

    [Header("Rotation")]
    public float angular_amplitude_update_rate;
    public float angular_frequency_update_rate;
    public Vector3 angular_amplitude;
    public Vector3 angular_frequency;
}



public class Test_BobAnimation : MonoBehaviour
{
    protected float x_time, y_time, angular_time; // Stores the normalized angle of the wave (0 -> 0; 0.25f -> PI/2; 0.5f -> PI; 0.75f -> 3PI/2; 1f -> 2PI)
    protected float x_this_frame, y_this_frame, a_this_frame; // Stores the parameters to pass to Sin(x) and Cos(y) functions every frame
    protected float sin_this_frame, cos_this_frame, a_sin_this_frame; // Stores the result of Sin(x) and Cos(y) operations every frame.
    protected float current_sin_amplitude, current_cos_amplitude, current_a_sin_amplitude;

    [HideInInspector] public Vector3 displacement, angular_displacement; // The total displacement at a given angle of the cicle.

    [Header("Bob Settings")]
    public MovementState bob_type = MovementState.IDLE;

    [Header("Sin(x)")]
    public float sin_amplitude = 1f; // The "size" of the wave.
    public float x_frequency = 1f; // The "speed" of the wave.

    [Header("Cos(y)")]
    public float walking_cos_amplitude = 1;
    public float cos_amplitude = 1f;
    public float y_frequency = 1f;

    [Header("Sin(angular)")]
    public float angular_frequency = 1f; // Sugestion: Different frequences for each axis.
    public Vector3 angular_sin_amplitude;

    public FBobAnimationSettings animation_settings;

    FBobAnimationSettings current_settings, target_settings;

    float target_x_amplitude, target_y_amplitude, target_angular_amplitude;
    float target_x_frequency, target_y_frequency, target_angular_frequency;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            add_animation(animation_settings);
        }

        //update(Time.deltaTime);
    }

    public void add_animation(FBobAnimationSettings settings)
    {
        target_settings = settings;
    }

    // Update is called once per frame
    public void update(float dt, MovementState movement_state)
    {
        {
            if (movement_state == MovementState.IDLE) current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, cos_amplitude, Time.deltaTime * y_frequency);
            else if (movement_state == MovementState.WALKING) current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, walking_cos_amplitude, Time.deltaTime * y_frequency);
        }

        { // Updates the animation
            //sin_time = increase_wave_time(sin_time, current_sin_frequency, dt);
            //cos_time = increase_wave_time(cos_time, current_cos_frequency, dt);
            //a_sin_time = increase_wave_time(a_sin_time, current_a_sin_frequency, dt);

            x_time = increase_wave_time(x_time, x_frequency, dt);
            y_time = increase_wave_time(y_time, y_frequency, dt);
            angular_time = increase_wave_time(angular_time, angular_frequency, dt);
        }

        { // Calculates the sin / cos
            sin_this_frame = Mathf.Sin(GI.player.tau * x_time + 0f);
            cos_this_frame = Mathf.Cos(2f * GI.player.tau * y_time + 0f);
            a_sin_this_frame = Mathf.Sin(GI.player.tau * angular_time + 0f);
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
