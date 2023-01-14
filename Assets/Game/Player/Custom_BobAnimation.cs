using UnityEngine;

//public enum Quadrant
//{
//    NONE,
//    FIRST,
//    SECOND,
//    THIRD,
//    FOURTH,
//}

public class Custom_BobAnimation : MonoBehaviour
{
    // "a" means "angular". Ex: "a_sin_time" means "angular_sin_time"
    float tau;
    float sin_time, cos_time, a_sin_time;
    float x_this_frame, y_this_frame, a_this_frame;
    float sin_this_frame, cos_this_frame, a_sin_this_frame;
    bool has_moved_last_frame;
    Quadrant sin_quadrant_on_key_up, cos_quadrant_on_key_up, a_sin_quadrant_on_key_up;
    Vector3 displacement, angular_displacement, start_position, start_rotation;

    [Header("Sin(x)")]
    public float sin_amplitude = 1f;
    public float sin_frequency = 1f;

    [Header("Cos(y)")]
    public float cos_amplitude = 1f;
    public float cos_frequency = 1f;

    [Header("Sin(angular)")]
    public float angular_sin_frequency = 1f;
    public Vector3 angular_sin_amplitude;

    public void init()
    {
        tau = 2 * Mathf.PI;
        start_position = transform.localPosition;
        start_rotation = transform.localEulerAngles;
        sin_quadrant_on_key_up = cos_quadrant_on_key_up = a_sin_quadrant_on_key_up = Quadrant.FIRST;
    }

    // Update is called once per frame
    public void update(float dt, bool is_moving)
    {
        { // Handle the animation based on the player's input
            if (is_moving) // Updates the animation over time
            {
                sin_time = increase_wave_time(sin_time, sin_frequency, dt);
                cos_time = increase_wave_time(sin_time, sin_frequency, dt);
                a_sin_time = increase_wave_time(a_sin_time, angular_sin_frequency, dt);
            }
            else if (has_moved_last_frame && !is_moving) // Check where the animation is, on the wave cicle, and set the "return path" based on the cicle.
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
            x_this_frame = tau * sin_time + 0f;
            y_this_frame = tau * cos_time + 0f;
            a_this_frame = tau * a_sin_time + 0f;

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
            displacement.x = sin_amplitude * sin_this_frame;
            displacement.y = cos_amplitude * cos_this_frame;

            angular_displacement.x = angular_sin_amplitude.x * a_sin_this_frame;
            angular_displacement.y = angular_sin_amplitude.y * a_sin_this_frame;
            angular_displacement.z = angular_sin_amplitude.z * a_sin_this_frame;
        }

        { // Updates the gameObject position and rotation.
            transform.localPosition = start_position + displacement;
            transform.localPosition -= Vector3.up * cos_amplitude; // Corrects the Y position to be the start position on "cos(0)".

            transform.localRotation = Quaternion.Euler(start_rotation + angular_displacement);
        }

        has_moved_last_frame = is_moving;
    }

    public float increase_wave_time(float wave_time, float wave_frequency, float dt)
    {
        wave_time += dt * wave_frequency;
        if (wave_time >= 1f) wave_time -= 1f; // On "time == 1f", the wave completes the 360° cicle. The value is being reseted to don't trepass the float number limit.

        return wave_time;

        // Using "Time.deltaTime" instead of "Time.time" because the cicle resets after "time == 1".

        // Time and Frequency are being multiplied here and stored into a variable to "normalize" the wave cicle at "1".
        // If they were separeted, it would be harder to check if the wave cicle was completed,
        // and it would be even harder to RESET the cicle, because it would be need to reset the "time" and "frequency" separately.
    }

    public float reset_wave_time(float wave_time, float wave_frequency, float dt, Quadrant wave_quadrant)
    {
        if (wave_quadrant == Quadrant.THIRD || wave_quadrant == Quadrant.FOURTH)
        {
            wave_time += dt * wave_frequency;
            if (wave_time > 1f) wave_time = 0f;
        }
        //else if (wave_quadrant == Quadrant.FIRST || wave_quadrant == Quadrant.SECOND)
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

        // Theorically, 0, PI, PI/2, 3PI/2 and 2PI are not in any quadrant.
        // But, assign then to a quadrant will not destroy anything and will make the code simple.
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
