using UnityEngine;

//public enum Quadrant
//{
//    NONE,
//    FIRST,
//    SECOND,
//    THIRD,
//    FOURTH,
//}

public enum WaveType
{
    SIN,
    COS,
    ANGULAR_SIN,
}

public class CameraShake : MonoBehaviour
{
    float shaking_t;
    bool shaking, has_shaked_last_frame;

    // "a" means "angular". Ex: "a_sin_time" means "angular_sin_time"
    float sin_time, cos_time, a_sin_time;
    float x_this_frame, y_this_frame, a_this_frame;
    float sin_this_frame, cos_this_frame, a_sin_this_frame;
    float current_sin_amplitude, current_cos_amplitude;
    Quadrant sin_quadrant_on_key_up, cos_quadrant_on_key_up, a_sin_quadrant_on_key_up;
    [HideInInspector] public Vector3 displacement, angular_displacement, current_a_sin_amplitude;

    [Header("Shake")]
    public float shake_time;

    [Header("Sin(x)")]
    public bool randomize_start_sin_amplitude;
    public float sin_amplitude = 1f;
    public float sin_frequency = 1f;

    [Header("Cos(2y)")]
    public bool randomize_start_cos_amplitude;
    public float cos_amplitude = 1f;
    public float cos_frequency = 1f;

    [Header("Sin(angular)")]
    public bool randomize_start_a_sin_amplitude;
    public float angular_sin_frequency = 1f;
    public Vector3 angular_sin_amplitude;

    public void add_shake()
    {
        shaking_t = shake_time;
        shaking = true;

        current_sin_amplitude = sin_amplitude;
        if (randomize_start_sin_amplitude) current_sin_amplitude *= Random.Range(0, 2) == 0 ? -1 : 1;

        current_cos_amplitude = cos_amplitude;
        if (randomize_start_cos_amplitude) current_cos_amplitude *= Random.Range(0, 2) == 0 ? -1 : 1;

        current_a_sin_amplitude.x = angular_sin_amplitude.x;
        current_a_sin_amplitude.y = angular_sin_amplitude.y;
        current_a_sin_amplitude.z = angular_sin_amplitude.z;
        if (randomize_start_a_sin_amplitude)
        {
            current_a_sin_amplitude.x *= Random.Range(0, 2) == 0 ? -1 : 1;
            current_a_sin_amplitude.y *= Random.Range(0, 2) == 0 ? -1 : 1;
            current_a_sin_amplitude.z *= Random.Range(0, 2) == 0 ? -1 : 1;
        }
    }

    public void update(float dt)
    {
        { // Handle the animation based on the player's input
            if (shaking) // Updates the animation over time
            {
                sin_time = increase_wave_time(sin_time, sin_frequency, dt, WaveType.SIN);
                cos_time = increase_wave_time(cos_time, cos_frequency, dt, WaveType.COS);
                a_sin_time = increase_wave_time(a_sin_time, angular_sin_frequency, dt, WaveType.ANGULAR_SIN);

                shaking_t -= dt;
                if (shaking_t <= 0f) shaking = false;
            }
            else if (!shaking && has_shaked_last_frame) // Check where the animation is, on the wave cicle, and set the "return path" based on the cicle.
            {
                sin_quadrant_on_key_up = get_quadrant_of_current_wave(sin_time);
                cos_quadrant_on_key_up = get_quadrant_of_current_wave(cos_time);
                a_sin_quadrant_on_key_up = get_quadrant_of_current_wave(a_sin_time);

                if (sin_quadrant_on_key_up == Quadrant.THIRD) sin_time = update_third_quadrant_to_fourth(sin_time);
                else if (sin_quadrant_on_key_up == Quadrant.SECOND) sin_time = update_second_quadrant_to_first(sin_time);

                if (cos_quadrant_on_key_up == Quadrant.THIRD) cos_time = update_third_quadrant_to_fourth(cos_time);
                else if (cos_quadrant_on_key_up == Quadrant.SECOND) cos_time = update_second_quadrant_to_first(cos_time);

                if (a_sin_quadrant_on_key_up == Quadrant.THIRD) a_sin_time = update_third_quadrant_to_fourth(a_sin_time);
                else if (a_sin_quadrant_on_key_up == Quadrant.SECOND) a_sin_time = update_second_quadrant_to_first(a_sin_time);
            }
            else // resets the wave cicle
            {
                if (sin_time > 0f) sin_time = reset_wave_time(sin_time, sin_frequency, dt, sin_quadrant_on_key_up);
                if (cos_time > 0f) cos_time = reset_wave_time(cos_time, cos_frequency, dt, cos_quadrant_on_key_up);
                if (a_sin_time > 0f) a_sin_time = reset_wave_time(a_sin_time, angular_sin_frequency, dt, a_sin_quadrant_on_key_up);
            }
        }

        { // Calculates the sin / cos
            x_this_frame = GI.player.tau * sin_time + 0f;
            y_this_frame = GI.player.tau * cos_time + 0f;
            a_this_frame = GI.player.tau * a_sin_time + 0f;

            // asin(x)
            // asin(2PI*t*f + 0)
            // a * sin(2PI * t * f + 0)
            // amplitude * Mathf.Sin(2 * Mathf.PI * Time.time * frequency + 0)
            // "0" means the "wave phase". It sets in which quadrant the wave will start.
            // The "wave phase" must be set in radian degrees: 0, PI, PI/2, 3PI/2 or 2PI.
            sin_this_frame = Mathf.Sin(x_this_frame);
            cos_this_frame = Mathf.Cos(2f * y_this_frame);
            a_sin_this_frame = Mathf.Sin(a_this_frame);
        }

        { // Sets the displacement of the position and rotation base on the sin / cos.
            displacement.x = current_sin_amplitude * sin_this_frame;
            displacement.y = current_cos_amplitude * cos_this_frame - current_cos_amplitude;

            angular_displacement.x = current_a_sin_amplitude.x * a_sin_this_frame;
            angular_displacement.y = current_a_sin_amplitude.y * a_sin_this_frame;
            angular_displacement.z = current_a_sin_amplitude.z * a_sin_this_frame;
        }

        has_shaked_last_frame = shaking;
    }

    public float increase_wave_time(float wave_time, float wave_frequency, float dt, WaveType wave_type)
    {
        wave_time += dt * wave_frequency;
        if (wave_time >= 1f)  // Resets the cicle
        { 
            wave_time -= 1f; // On "time == 1f", the wave completes the 360° cicle. The value is being reseted to don't trepass the float number limit.
            switch (wave_type) // Invert the amplitude
            {
                case WaveType.SIN: current_sin_amplitude *= -1f; break;
                case WaveType.COS: current_cos_amplitude *= -1f; break;
                case WaveType.ANGULAR_SIN:
                    {
                        current_a_sin_amplitude.x *= -1f;
                        current_a_sin_amplitude.y *= -1f;
                        current_a_sin_amplitude.z *= -1f;
                    }
                    break;
            }
        }

        return wave_time;
    }

    public float reset_wave_time(float wave_time, float wave_frequency, float dt, Quadrant wave_quadrant)
    {
        if (wave_quadrant == Quadrant.THIRD || wave_quadrant == Quadrant.FOURTH)
        {
            wave_time += dt * wave_frequency;
            if (wave_time > 1f) wave_time = 1f;
        }
        else
        {
            wave_time -= dt * wave_frequency;
            if (wave_time < 0f) wave_time = 0f;
        }

        return wave_time;
    }

    public Quadrant get_quadrant_of_current_wave(float wave_time)
    {
        if (wave_time >= .75f) return Quadrant.FOURTH;
        else if (wave_time >= .5f) return Quadrant.THIRD;
        else if (wave_time >= .25f) return Quadrant.SECOND;
        else return Quadrant.FIRST;
    }

    public float update_third_quadrant_to_fourth(float wave_time)
    {
        if (wave_time < .5f || wave_time >= .75f) return wave_time;

        wave_time = 1f - wave_time; // Returns the time needed to reset the cicle. Ex: 1f - 0.6f == 0.4f
        wave_time += 0.5f; // Returns the time needed to reset the FOURTH quadrant. Ex: 0.4f + 0.5f == 0.9f

        return wave_time;
    }

    public float update_second_quadrant_to_first(float wave_time)
    {
        if (wave_time < .25f || wave_time >= .5f) return wave_time;

        return 0.5f - wave_time;
    }
}
