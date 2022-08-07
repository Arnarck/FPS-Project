using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    bool is_turned_on, has_battery = true;

    [SerializeField] Light flashlight;
    [SerializeField] float battery_consumed_per_frame;
    public float battery = 100f;

    void Awake()
    {
        GI.player_flashlight = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Flashlight input
            if (Input.GetKeyDown(KeyCode.L) && has_battery)
            {
                is_turned_on = !is_turned_on;
                flashlight.gameObject.SetActive(is_turned_on);
            }
        }

        { // Consume battery
            if (is_turned_on && has_battery)
            {
                battery -= Time.deltaTime * battery_consumed_per_frame;
                if (battery < Mathf.Epsilon)
                {
                    battery = 0f;
                    has_battery = false;
                    is_turned_on = false;
                    flashlight.gameObject.SetActive(false);
                }
            }
        }
    }

    public bool add_battery(float value)
    {
        if (value < 0) value *= -1f;
        if (battery >= 100f) return false; // Battery is already full

        battery += value;
        has_battery = true;
        battery = Mathf.Clamp(battery, 0f, 100f);
        return true;
    }
}
