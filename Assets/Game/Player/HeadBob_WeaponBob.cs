//using UnityEngine;

//public class HeadBob_WeaponBob : BobAnimation
//{
//    bool has_moved_last_frame;

//    [Header("Running Sin(x)")]
//    public float running_sin_amplitude = 2f;
//    public float running_sin_frequency = 2f;

//    [Header("Running Cos(y)")]
//    public float running_cos_amplitude = 2f;
//    public float running_cos_frequency = 2f;

//    [Header("Running Sin(angular)")]
//    public float running_angular_sin_frequency = 1f;
//    public Vector3 running_angular_sin_amplitude;

//    [Header("Amplitude Update Over Time")]
//    public float amplitude_lerp_factor = 3f;

//    // Update is called once per frame
//    public void update(float dt, MovementState movement_state)
//    {
//        // Set amplitude and frequency based on movement state


//        { // Handle the animation based on the player's input
//            if (movement_state != MovementState.IDLE) // Updates the animation over time if is moving
//            {
//                if (movement_state == MovementState.WALKING)
//                {
//                    current_sin_frequency = sin_frequency;
//                    current_sin_amplitude = Mathf.Lerp(current_sin_amplitude, sin_amplitude, dt * current_sin_frequency * amplitude_lerp_factor);

//                    current_cos_frequency = cos_frequency;
//                    current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, cos_amplitude, dt * current_cos_frequency * amplitude_lerp_factor);

//                    current_a_sin_frequency = angular_sin_frequency;
//                    current_a_sin_amplitude = Vector3.Lerp(current_a_sin_amplitude, angular_sin_amplitude, dt * current_a_sin_frequency * amplitude_lerp_factor);
//                }
//                else if (movement_state == MovementState.RUNNING)
//                {
//                    current_sin_frequency = running_sin_frequency;
//                    current_sin_amplitude = Mathf.Lerp(current_sin_amplitude, running_sin_amplitude, dt * current_sin_frequency * amplitude_lerp_factor);

//                    current_cos_frequency = running_cos_frequency;
//                    current_cos_amplitude = Mathf.Lerp(current_cos_amplitude, running_cos_amplitude, dt * current_cos_frequency * amplitude_lerp_factor);

//                    current_a_sin_frequency = running_angular_sin_frequency;
//                    current_a_sin_amplitude = Vector3.Lerp(current_a_sin_amplitude, running_angular_sin_amplitude, dt * current_a_sin_frequency * amplitude_lerp_factor);

//                }

//                update_animation(dt);
//            }
//            else if (has_moved_last_frame && movement_state == MovementState.IDLE) // Check where the animation is, on the wave cicle, and set the "return path" based on the cicle.
//            {
//                set_return_path_based_on_current_quadrant();
//            }
//            else // resets the wave cicle if not moving
//            {
//                reset_wave_cicle(dt);
//            }
//        }

//        { // Calculates the sin / cos
//            calculate_sine_and_cosine();
//        }

//        { // Sets the displacement of the position and rotation base on the sin / cos.
//            set_displacement_based_on_sine_and_cosine();
//        }

//        { // Updates the gameObject position and rotation.
//            update_gameobject_coordinates();
//        }

//        has_moved_last_frame = movement_state != MovementState.IDLE;
//    }
//}
