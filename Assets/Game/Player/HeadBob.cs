using UnityEngine;

public class HeadBob : BobAnimation
{
    bool has_moved_last_frame;
    [Header("Head Bob Settings")]
    public MovementState bob_type = MovementState.IDLE;

    // Update is called once per frame
    public void update(float dt, MovementState movement_state)
    {
        { // Handle the animation based on the player's input
            if (movement_state == bob_type) // Updates the animation over time if is moving
            {
                update_animation(dt);
            }
            else if (has_moved_last_frame && movement_state != bob_type) // Check where the animation is, on the wave cicle, and set the "return path" based on the cicle.
            {
                set_return_path_based_on_current_quadrant();
            }
            else // resets the wave cicle if not moving
            {
                reset_wave_cicle(dt);
            }
        }

        { // Calculates the sin / cos
            calculate_sine_and_cosine();
        }

        { // Sets the displacement of the position and rotation base on the sin / cos.
            set_displacement_based_on_sine_and_cosine();
        }

        { // Updates the gameObject position and rotation.
            //update_gameobject_coordinates();
        }

        has_moved_last_frame = movement_state == bob_type;
    }
}
