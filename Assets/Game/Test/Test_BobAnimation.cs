using UnityEngine;



[System.Serializable]
public struct FBobAnimationSettings
{
    [Header("Blending Animations")]
    [Tooltip("How fast the previous animation will blend to this new one.")] public float blend_amplitude;
    public float blend_frequency;
    public float blend_angular_amplitude;
    public float blend_angular_frequency;

    [Header("Directional")]
    public Vector3 amplitude;
    public Vector3 frequency;

    [Header("Rotation")]
    public Vector3 angular_amplitude;
    public Vector3 angular_frequency;
}



public class Test_BobAnimation : MonoBehaviour
{
    Vector3 unit_circle, angular_unit_circle; // Stores the normalized angle of the wave (0 -> 0; 0.25f -> PI/2; 0.5f -> PI; 0.75f -> 3PI/2; 1f -> 2PI)
    FBobAnimationSettings current, target;

    [HideInInspector] public Vector3 displacement, angular_displacement; // The total displacement at a given angle of the cicle.
    public FBobAnimationSettings animation_settings;
    public FBobAnimationSettings i_settings;
    public FBobAnimationSettings r_settings;

    private void Update()
    {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            animate(r_settings);
        }
        else if (Input.GetKey(KeyCode.W))
        {
            animate(animation_settings);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            animate(i_settings);
        }

        update(Time.deltaTime);
    }

    public void animate(FBobAnimationSettings settings)
    {
        target = settings;
    }

    // Update is called once per frame
    public void update(float dt)
    {
        { // Blend animations
            current.amplitude = Vector3.Lerp(current.amplitude, target.amplitude, dt * target.blend_amplitude);
            current.frequency = Vector3.Lerp(current.frequency, target.frequency, dt * target.blend_frequency);

            current.angular_amplitude = Vector3.Lerp(current.angular_amplitude, target.angular_amplitude, dt * target.blend_angular_amplitude);
            current.angular_frequency = Vector3.Lerp(current.angular_frequency, target.angular_frequency, dt * target.blend_angular_frequency);
        }

        { // Updates the animation
            for (int i = 0; i < 3; i++)
            {
                unit_circle[i] += dt * current.frequency[i];
                if (unit_circle[i] >= 1f) unit_circle[i] -= 1f;

                angular_unit_circle[i] += dt * current.angular_frequency[i];
                if (angular_unit_circle[i] >= 1f) angular_unit_circle[i] -= 1f;
            }
        }

        { // Calculates the displacement
            displacement.x = current.amplitude.x * Mathf.Sin(2f * Mathf.PI * unit_circle.x + 0f);
            displacement.y = current.amplitude.y * Mathf.Cos(2f * (2f * Mathf.PI) * unit_circle.y + 0f) - current.amplitude.y;
            displacement.z = current.amplitude.z * Mathf.Sin(2f * Mathf.PI * unit_circle.z + 0f);

            angular_displacement.x = current.angular_amplitude.x * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.x + 0f);
            angular_displacement.y = current.angular_amplitude.y * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.y + 0f);
            angular_displacement.z = current.angular_amplitude.z * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.z + 0f);
        }

        { // Sets the displacement - ONLY FOR TEST
            transform.position = displacement;
            transform.rotation = Quaternion.Euler(angular_displacement);
        }
    }
}
