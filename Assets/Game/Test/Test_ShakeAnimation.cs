using UnityEngine;



public enum ECircleQuadrant
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

    [Header("Directional Shake")]
    public Vector3 amplitude;
    public Vector3 frequency;
    public bool[] randomize;

    [Header("Angular Shake")]
    public Vector3 angular_frequency;
    public Vector3 angular_amplitude;
    public bool[] randomize_rotation;
}



public class Test_ShakeAnimation : MonoBehaviour
{
    float duration_t;
    bool shaking;
    ECircleQuadrant[] quadrant_on_animation_end = new ECircleQuadrant[3], angular_quadrant_on_animation_end = new ECircleQuadrant[3]; // 0 - x; 1 - y; z - 2
    Vector3 unit_circle, angular_unit_circle;
    FShakeAnimationSettings shake;

    [HideInInspector] public Vector3 displacement, angular_displacement;

    public FShakeAnimationSettings shake_settings;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            add_shake(shake_settings);
        }

        update(Time.deltaTime);
    }

    public void add_shake(FShakeAnimationSettings settings, bool reset_duration_if_already_shaking = true)
    {
        if (shaking)
        {
            if (reset_duration_if_already_shaking) duration_t = shake.duration;
        }
        else // Start shaking
        {
            shaking = true;
            shake = settings;
            duration_t = shake.duration;
            for (int i = 0; i < 3; i++) // Only randomize the direction if the player wasn't previously shaking.
            {
                if (i < shake.randomize.Length && shake.randomize[i]) shake.amplitude[i] *= Random.Range(0, 2) == 0 ? -1 : 1;
                if (i < shake.randomize_rotation.Length && shake.randomize_rotation[i]) shake.angular_amplitude[i] *= Random.Range(0, 2) == 0 ? -1 : 1;
            }
        }
    }

    public void update(float dt)
    {
        if (!shaking) return;

        { // Animation
            if (duration_t > 0f) // Updates the animation over time
            {
                unit_circle += dt * shake.frequency;
                angular_unit_circle += dt * shake.angular_frequency;
                for (int i = 0; i < 3; i++)
                {
                    // On "time == 1f", the wave completes the 360° cicle. The value is being reseted to don't trepass the float number limit.
                    if (unit_circle[i] >= 1f) unit_circle[i] -= 1f;
                    if (angular_unit_circle[i] >= 1f) angular_unit_circle[i] -= 1f;
                }

                duration_t -= dt;
                if (duration_t <= 0f) 
                {
                    for (int i = 0; i < 3; i++) // Check where the wave cicle is, and set the "shortest return path".
                    {
                        quadrant_on_animation_end[i] = get_circle_quadrant(unit_circle[i]);
                        if (quadrant_on_animation_end[i] == ECircleQuadrant.THIRD) unit_circle[i] = third_quadrant_to_fourth(unit_circle[i]);
                        else if (quadrant_on_animation_end[i] == ECircleQuadrant.SECOND) unit_circle[i] = second_quadrant_to_first(unit_circle[i]);

                        angular_quadrant_on_animation_end[i] = get_circle_quadrant(angular_unit_circle[i]);
                        if (angular_quadrant_on_animation_end[i] == ECircleQuadrant.THIRD) angular_unit_circle[i] = third_quadrant_to_fourth(angular_unit_circle[i]);
                        else if (angular_quadrant_on_animation_end[i] == ECircleQuadrant.SECOND) angular_unit_circle[i] = second_quadrant_to_first(angular_unit_circle[i]);
                    }
                }
            }

            if (duration_t <= 0f) // Finishes the animation (can happen on the same frame as the animation)
            {
                shaking = false;
                for (int i = 0; i < 3; i++)
                {
                    if (unit_circle[i] > 0f && unit_circle[i] < 1f)
                    {
                        shaking = true;
                        if (quadrant_on_animation_end[i] == ECircleQuadrant.THIRD || quadrant_on_animation_end[i] == ECircleQuadrant.FOURTH)
                        {
                            unit_circle[i] += dt * shake.frequency[i];
                            if (unit_circle[i] > 1f) unit_circle[i] = 1f;
                        }
                        else
                        {
                            unit_circle[i] -= dt * shake.frequency[i];
                            if (unit_circle[i] < 0f) unit_circle[i] = 0f;
                        }
                    }

                    if (angular_unit_circle[i] > 0f && angular_unit_circle[i] < 1f)
                    {
                        shaking = true;
                        if (angular_quadrant_on_animation_end[i] == ECircleQuadrant.THIRD || angular_quadrant_on_animation_end[i] == ECircleQuadrant.FOURTH)
                        {
                            angular_unit_circle[i] += dt * shake.angular_frequency[i];
                            if (angular_unit_circle[i] > 1f) angular_unit_circle[i] = 1f;
                        }
                        else
                        {
                            angular_unit_circle[i] -= dt * shake.angular_frequency[i];
                            if (angular_unit_circle[i] < 0f) angular_unit_circle[i] = 0f;
                        }
                    }
                }
            }
        }

        { // Calculates the displacement
            // asin(x)
            // asin(2PI*t*f + 0)
            // a * sin(2PI * t * f + 0)
            // amplitude * Mathf.Sin(2 * Mathf.PI * Time.time * frequency + 0)
            // "0" means the "wave phase". It sets in which quadrant the wave will start.
            // The "wave phase" must be set in radian degrees: 0, PI, PI/2, 3PI/2 or 2PI.
            displacement.x = shake.amplitude.x * Mathf.Sin((2 * Mathf.PI) * unit_circle.x + 0f);
            displacement.y = shake.amplitude.y * Mathf.Cos(2f * ((2 * Mathf.PI) * unit_circle.y + 0f)) - shake.amplitude.y;
            displacement.z = shake.amplitude.z * Mathf.Sin((2 * Mathf.PI) * unit_circle.z + 0f);

            angular_displacement.x = shake.angular_amplitude.x * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.x + 0f);
            angular_displacement.y = shake.angular_amplitude.y * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.y + 0f);
            angular_displacement.z = shake.angular_amplitude.z * Mathf.Sin(2f * Mathf.PI * angular_unit_circle.z + 0f);
        }

        { // Sets displacement - ONLY FOR TEST
            transform.localPosition = displacement;
            transform.localRotation = Quaternion.Euler(angular_displacement);
        }

    }

    public ECircleQuadrant get_circle_quadrant(float unit_circle)
    {
        if (unit_circle >= .75f) return ECircleQuadrant.FOURTH;
        else if (unit_circle >= .5f) return ECircleQuadrant.THIRD;
        else if (unit_circle >= .25f) return ECircleQuadrant.SECOND;
        else return ECircleQuadrant.FIRST;
    }

    public float third_quadrant_to_fourth(float unit_circle)
    {
        if (unit_circle < .5f || unit_circle >= .75f) return unit_circle;

        unit_circle = 1f - unit_circle; // Returns the time needed to reset the cicle. Ex: 1f - 0.6f == 0.4f
        unit_circle += 0.5f; // Returns the time needed to reset the FOURTH quadrant. Ex: 0.4f + 0.5f == 0.9f

        return unit_circle;
    }

    public float second_quadrant_to_first(float unit_circle)
    {
        if (unit_circle < .25f || unit_circle >= .5f) return unit_circle;

        return 0.5f - unit_circle;
    }
}
