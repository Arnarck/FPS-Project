using UnityEngine;



public enum EWaveQuadrant
{
    NONE,
    FIRST,
    SECOND,
    THIRD,
    FOURTH,
}

public enum EWaveType
{
    SIN,
    COS,
    ANGULAR_SIN,
}

[System.Serializable]
public struct FShakeAnimationSettings
{
    public float duration;

    [Header("X Axis")]
    public bool x_randomize_start_movement;
    public float x_amplitude;
    public float x_frequency;

    [Header("Y Axis")]
    public bool y_randomize_start_movement;
    public float y_amplitude;
    public float y_frequency;

    [Header("Z Axis")]
    public bool z_randomize_start_movement;
    public float z_amplitude;
    public float z_frequency;

    [Header("Rotation")]
    public bool angular_randomize_start_movement;
    public float angular_frequency;
    public Vector3 angular_amplitude;
}



public class Test_ShakeAnimation : MonoBehaviour
{
    float duration_t;
    bool shaking;
    float x_animation_time, y_animation_time, z_animation_time, angular_animation_time;
    EWaveQuadrant x_animation_on_key_up, y_animation_on_key_up, z_animation_on_key_up, angular_animation_on_key_up;
    FShakeAnimationSettings current_settings;

    [HideInInspector] public Vector3 displacement, angular_displacement;

    public FShakeAnimationSettings settings;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            add_shake();
        }

        update(Time.deltaTime);
    }

    public void add_shake()
    {
        duration_t = settings.duration;

        if (!shaking) // Only randomize the direction if the player wasn't previously shaking.
        {
            current_settings.x_amplitude = settings.x_amplitude;
            if (settings.x_randomize_start_movement) current_settings.x_amplitude *= Random.Range(0, 2) == 0 ? -1 : 1;

            current_settings.y_amplitude = settings.y_amplitude;
            if (settings.y_randomize_start_movement) current_settings.y_amplitude *= Random.Range(0, 2) == 0 ? -1 : 1;

            current_settings.z_amplitude = settings.z_amplitude;
            if (settings.z_randomize_start_movement) current_settings.z_amplitude *= Random.Range(0, 2) == 0 ? -1 : 1;

            current_settings.angular_amplitude.x = settings.angular_amplitude.x;
            current_settings.angular_amplitude.y = settings.angular_amplitude.y;
            current_settings.angular_amplitude.z = settings.angular_amplitude.z;

            if (settings.angular_randomize_start_movement)
            {
                current_settings.angular_amplitude.x *= Random.Range(0, 2) == 0 ? -1 : 1;
                current_settings.angular_amplitude.y *= Random.Range(0, 2) == 0 ? -1 : 1;
                current_settings.angular_amplitude.z *= Random.Range(0, 2) == 0 ? -1 : 1;
            }
        }

        shaking = true;
    }

    public void update(float dt)
    {
        { // Handle the animation based on the player's input
            if (shaking) // Updates the animation over time
            {
                x_animation_time = increase_animation_time(x_animation_time, settings.x_frequency, dt);
                y_animation_time = increase_animation_time(y_animation_time, settings.y_frequency, dt);
                z_animation_time = increase_animation_time(z_animation_time, settings.z_frequency, dt);
                angular_animation_time = increase_animation_time(angular_animation_time, settings.angular_frequency, dt);

                duration_t -= dt;
                if (duration_t <= 0f) 
                {
                    shaking = false;

                    // Check where the animation is, on the wave cicle, and set the "shortest return path" based on the location.
                    x_animation_on_key_up = get_animation_quadrant(x_animation_time);
                    y_animation_on_key_up = get_animation_quadrant(y_animation_time);
                    z_animation_on_key_up = get_animation_quadrant(z_animation_time);
                    angular_animation_on_key_up = get_animation_quadrant(angular_animation_time);

                    if (x_animation_on_key_up == EWaveQuadrant.THIRD) x_animation_time = third_quadrant_to_fourth(x_animation_time);
                    else if (x_animation_on_key_up == EWaveQuadrant.SECOND) x_animation_time = second_quadrant_to_first(x_animation_time);

                    if (y_animation_on_key_up == EWaveQuadrant.THIRD) y_animation_time = third_quadrant_to_fourth(y_animation_time);
                    else if (y_animation_on_key_up == EWaveQuadrant.SECOND) y_animation_time = second_quadrant_to_first(y_animation_time);

                    if (z_animation_on_key_up == EWaveQuadrant.THIRD) z_animation_time = third_quadrant_to_fourth(z_animation_time);
                    else if (z_animation_on_key_up == EWaveQuadrant.SECOND) z_animation_time = second_quadrant_to_first(z_animation_time);

                    if (angular_animation_on_key_up == EWaveQuadrant.THIRD) angular_animation_time = third_quadrant_to_fourth(angular_animation_time);
                    else if (angular_animation_on_key_up == EWaveQuadrant.SECOND) angular_animation_time = second_quadrant_to_first(angular_animation_time);
                }
            }
            
            if (!shaking) // Finishes the animation (can happen on the same frame as the animation)
            {
                if (x_animation_time > 0f) x_animation_time = reset_animation(x_animation_time, settings.x_frequency, dt, x_animation_on_key_up);
                if (y_animation_time > 0f) y_animation_time = reset_animation(y_animation_time, settings.y_frequency, dt, y_animation_on_key_up);
                if (z_animation_time > 0f) z_animation_time = reset_animation(z_animation_time, settings.z_frequency, dt, z_animation_on_key_up);
                if (angular_animation_time > 0f) angular_animation_time = reset_animation(angular_animation_time, settings.angular_frequency, dt, angular_animation_on_key_up);
            }
        }

        { // Calculates the displacement
            // asin(x)
            // asin(2PI*t*f + 0)
            // a * sin(2PI * t * f + 0)
            // amplitude * Mathf.Sin(2 * Mathf.PI * Time.time * frequency + 0)
            // "0" means the "wave phase". It sets in which quadrant the wave will start.
            // The "wave phase" must be set in radian degrees: 0, PI, PI/2, 3PI/2 or 2PI.
            displacement.x = current_settings.x_amplitude * Mathf.Sin((2 * Mathf.PI) * x_animation_time + 0f);
            displacement.y = current_settings.y_amplitude * Mathf.Cos(2f * ((2 * Mathf.PI) * y_animation_time + 0f)) - current_settings.y_amplitude;
            displacement.z = current_settings.z_amplitude * Mathf.Sin((2 * Mathf.PI) * z_animation_time + 0f);

            float raw_angular_displacement = Mathf.Sin((2 * Mathf.PI) * angular_animation_time + 0f);
            angular_displacement.x = current_settings.angular_amplitude.x * raw_angular_displacement;
            angular_displacement.y = current_settings.angular_amplitude.y * raw_angular_displacement;
            angular_displacement.z = current_settings.angular_amplitude.z * raw_angular_displacement;
        }

        { // Sets displacement - ONLY FOR TEST
            transform.localPosition = displacement;
            transform.localRotation = Quaternion.Euler(angular_displacement);
        }

    }

    public float increase_animation_time(float time, float frequency, float dt)
    {
        time += dt * frequency;
        if (time >= 1f)  // Resets the cicle
        {
            time -= 1f; // On "time == 1f", the wave completes the 360° cicle. The value is being reseted to don't trepass the float number limit.
        }

        return time;
    }

    public float reset_animation(float time, float frequency, float dt, EWaveQuadrant quadrant)
    {
        if (quadrant == EWaveQuadrant.THIRD || quadrant == EWaveQuadrant.FOURTH)
        {
            time += dt * frequency;
            if (time > 1f) time = 1f;
        }
        else
        {
            time -= dt * frequency;
            if (time < 0f) time = 0f;
        }

        return time;
    }

    public EWaveQuadrant get_animation_quadrant(float time)
    {
        if (time >= .75f) return EWaveQuadrant.FOURTH;
        else if (time >= .5f) return EWaveQuadrant.THIRD;
        else if (time >= .25f) return EWaveQuadrant.SECOND;
        else return EWaveQuadrant.FIRST;
    }

    public float third_quadrant_to_fourth(float time)
    {
        if (time < .5f || time >= .75f) return time;

        time = 1f - time; // Returns the time needed to reset the cicle. Ex: 1f - 0.6f == 0.4f
        time += 0.5f; // Returns the time needed to reset the FOURTH quadrant. Ex: 0.4f + 0.5f == 0.9f

        return time;
    }

    public float second_quadrant_to_first(float time)
    {
        if (time < .25f || time >= .5f) return time;

        return 0.5f - time;
    }
}
