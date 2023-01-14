using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    public float sin_time, cos_time;
    float tau;
    Vector3 displacement;
    Vector3 start_position;

    [Header("Sin(x)")]
    public float sin_multiplier = 1f;
    public float sin_amplitude = 1f;
    public float sin_frequency = 1f;

    [Header("Cos(x)")]
    public float cos_multiplier = 2f;
    public float cos_amplitude = 1f;
    public float cos_frequency = 1f;

    private void Start()
    {
        tau = 2 * Mathf.PI;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            // a sin(tau * t * f + 0)
            sin_time += Time.deltaTime * sin_frequency;
            cos_time += Time.deltaTime * cos_frequency;
            if (sin_time >= 1f) sin_time -= 1f;
            if (cos_time >= 1f) cos_time -= 1f;
        }
        else 
        { 
            sin_time -= Time.deltaTime * sin_frequency;
            cos_time -= Time.deltaTime * cos_frequency;
            if (sin_time < 0f) sin_time = 0f;
            if (cos_time < 0f) cos_time = 0f;
        }


        float x = tau * sin_time + 0;
        float y = tau * cos_time + 0;

        displacement.x = sin_amplitude * Mathf.Sin(sin_multiplier * x);
        displacement.y = cos_amplitude * Mathf.Cos(cos_multiplier * y);

        transform.localPosition = displacement - Vector3.up;



        //if (Input.GetKey(KeyCode.W))
        //{
        //    x_time += Time.deltaTime;
        //    y_time -= Time.deltaTime;

        //    if (x_time >= 1f) x_time -= 1f;
        //    if (y_time <= 0f) y_time += 1f;
        //}
        //else
        //{
        //    x_time -= Time.deltaTime;
        //    y_time += Time.deltaTime;

        //    if (x_time <= 0f) x_time = 0f;
        //    if (y_time <= 0f) x_time = 0f;
        //}

        //Vector3 position = Vector3.zero;
        //position.x = amplitude * Mathf.Sin(2*Mathf.PI * x_time * frequency + 0f);
        //position.y = amplitude * Mathf.Sin(2*Mathf.PI * x_time * frequency + 0f);

        //transform.localPosition = position;


        //if (Input.GetKey(KeyCode.W))
        //{
        //    x_time += Time.deltaTime * speed;
        //    y_time += Time.deltaTime * speed;
        //}
        //else
        //{
        //    x_time -= Time.deltaTime * speed;
        //    y_time -= Time.deltaTime * speed;

        //    if (x_time <= 0f) x_time = 0f;
        //    if (y_time <= 0f) y_time = 0f;
        //}

        //{ // Updates x value over time
        //    x += Time.deltaTime * speed * direction;
        //    x = Mathf.Clamp(x, -radius, radius);

        //    if (x >= radius || x <= -radius) direction = -direction;
        //}

        //{ // Apply the weapon bob
        //    float y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2));
        //    y *= wave_crest;
        //    transform.position = start_position + new Vector3(x, y, 0f);
        //}

        //float sin = Mathf.Sin(x_time);
        //sin = Mathf.Clamp(sin, 0f, 1f);

        //if (sin <= 0f) x_time = 0f;
        //if (y_time >= 5f) y_time -= 5f;

        //Vector3 position = Vector3.zero;
        //position.x = radius * Mathf.Cos(y_time);
        //position.y = radius * sin;
        //transform.localPosition = position;
    }
}
