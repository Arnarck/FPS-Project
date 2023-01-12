using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBob : MonoBehaviour
{
    public Vector3 start_position;
    public float direction = 1;
    public float x;
    [Header("Weapon Bob")]
    public float wave_crest = 1f;
    public float radius = 5f;
    public float speed = 5f;

    [Header("Sin / Cos")]
    public float amplitude;
    public float frequency;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Mathf.Cos(0f));
        Debug.Log(Mathf.Cos(Mathf.PI / 2f));
        Debug.Log(Mathf.Cos(Mathf.PI));
        Debug.Log(Mathf.Cos( (3 * Mathf.PI) / 2f));
        Debug.Log(Mathf.Cos(2 * Mathf.PI));
    }

    float x_time;
    float y_time = 1f;

    // Update is called once per frame
    void Update()
    {
        x_time += Time.deltaTime;
        if (x_time >= 2 * Mathf.PI) x_time -= 2 * Mathf.PI;
        Debug.Log($"Sin: {Mathf.Sin(x_time)} | Cos: {Mathf.Cos(x_time * 2f)} | Time: {x_time}");
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
