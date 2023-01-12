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

    // Start is called before the first frame update
    void Start()
    {
        start_position = transform.localPosition;
        x = -radius;
    }

    // Update is called once per frame
    void Update()
    {
        { // Updates x value over time
            x += Time.deltaTime * speed * direction;
            x = Mathf.Clamp(x, -radius, radius);

            if (x >= radius || x <= -radius) direction = -direction;
        }

        { // Apply the weapon bob
            float y = Mathf.Sqrt(Mathf.Pow(radius, 2f) - Mathf.Pow(x, 2));
            y *= wave_crest;
            transform.position = start_position + new Vector3(x, y, 0f);
        }
    }
}
