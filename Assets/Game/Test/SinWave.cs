using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SinWavePhase
{
    ZERO = 0,
    PI_BY_TWO,
    THREE_PI_BY_TWO,
    TWO_PI,
}

public class SinWave : MonoBehaviour
{
    public float amplitude = 1;
    public float frequency = 1;
    public SinWavePhase sin_wave_phase;

    float time;

    // Update is called once per frame
    void Update()
    {
        // asin(2PIft+0)
        // a * sin(2PI * f * t + 0)
        // y = amplitude * sin(2PI * frequency * time + phase)
        time += Time.deltaTime * frequency;
        float x = 2 * Mathf.PI * time + get_sin_wave_phase_value();
        float y = amplitude * Mathf.Cos(x);
        transform.position = Vector3.up * y;

        if (time >= 1f) time -= 1f;
        Debug.Log($"Time: {time} | Cos: {y}");
    }

    public float get_sin_wave_phase_value()
    {
        switch (sin_wave_phase)
        {
            case SinWavePhase.ZERO:            return 0;
            case SinWavePhase.PI_BY_TWO:       return Mathf.PI / 2f;
            case SinWavePhase.THREE_PI_BY_TWO: return (3f * Mathf.PI) / 2f;
            case SinWavePhase.TWO_PI:          return Mathf.PI * 2f;
        }

        return 0f;
    }
}
