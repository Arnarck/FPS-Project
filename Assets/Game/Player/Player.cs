using UnityEngine;

public class Player : MonoBehaviour
{
    Vector3 camera_start_position;
    float time_elapsed_from_stamina_restoration, current_weapon_bob_location, weapon_bob_direction;
    [HideInInspector] public float fov_percentage = 1f, base_run_multiplier, current_run_multiplier, base_flashlight_range, base_flashlight_spot_angle, start_fov, head_bob_speed;
    [HideInInspector] Vector2 start_mouse_sensitivity;
    public bool is_alive = true, can_run = true, can_restore_stamina, is_overdosed;

    [Header("Health")]
    public float health = 50;
    public float max_health = 100f;

    [Header("Stamina")]
    public float stamina = 50;
    public float max_stamina = 100f;
    public float min_stamina_to_run_when_fatigued = 10f; // "Fatigued" is when the player is unable to run after using the stamina until the bar is empty.
    public float stamina_restored_per_frame = .5f;
    public float stamina_restored_per_frame_when_fatigued = 1f;
    public float stamina_consumed_per_frame = .5f;
    public float time_to_start_restoring_stamina = 1f;

    [Header("Terror")]
    public float terror;
    public float max_terror;
    public int current_terror_level;
    public float terror_level_1 = 30;
    public float terror_level_2 = 55;
    public float terror_level_3 = 75;
    public float run_multiplier_on_terror_level_1;
    public float run_multiplier_on_terror_level_2;
    public float run_multiplier_on_terror_level_3;

    [Header("Overdose")]
    public float overdose;
    public float max_overdose;
    public float overdose_reduced_per_frame = 15f;
    public float overdose_reduced_per_frame_when_overdosed = 5f;

    [Header("Aim")]
    public bool is_aiming;
    public float fov_speed = 10f;
    public float aiming_flashlight_range = 30f;
    public float aiming_flashlight_spot_angle = 40f;

    [Header("Flashlight")]
    public Light flashlight;

    [Header("Head Bob")]
    public float amplitude = 1; // The "height" of the wave.
    public float frequency = 1; // The "speed" of the wave.

    [Header("Weapon Bob")]
    public float weapon_bob_radius = .1f;
    public Transform weapon_holster;
    

    void Awake()
    {
        GI.player = this;
        GI.fp_camera = Camera.main;
    }

    void Start()
    {
        GI.hud.health_bar.maxValue = max_health;
        GI.hud.health_bar.value = health;

        GI.hud.stamina_bar.maxValue = max_stamina;
        GI.hud.stamina_bar.value = stamina;

        GI.hud.terror_bar.maxValue = max_terror;
        GI.hud.terror_bar.value = terror;

        GI.hud.overdose_bar.maxValue = max_overdose;
        GI.hud.overdose_bar.value = overdose;

        start_fov = GI.fp_camera.fieldOfView;

        base_run_multiplier = GI.fp_controller.movementSettings.RunMultiplier;

        start_mouse_sensitivity.x = GI.fp_controller.mouseLook.XSensitivity;
        start_mouse_sensitivity.y = GI.fp_controller.mouseLook.YSensitivity;

        GI.hud.ammo_display.SetActive(GI.player_inventory.is_equiped_with_a_gun());
        GI.hud.display_crosshair_of_equiped_weapon();

        base_flashlight_range = flashlight.range;
        base_flashlight_spot_angle = flashlight.spotAngle;

        camera_start_position = Camera.main.transform.localPosition;

        weapon_bob_direction = weapon_bob_radius < 0f ? -1f : 1f;
    }

    void FixedUpdate()
    {
        if (GI.pause_game.game_paused) return;

        { // Stamina
            if (GI.fp_controller.Running) // Consume stamina
            {
                time_elapsed_from_stamina_restoration = 0f; // Resets the counter
                can_restore_stamina = false;

                stamina -= stamina_consumed_per_frame;
                stamina = Mathf.Clamp(stamina, 0, max_stamina);
                GI.hud.stamina_bar.value = stamina;

                if (stamina < Mathf.Epsilon) can_run = false;
            }
            else if (!can_restore_stamina) // Cooldown to restore stamina
            {
                time_elapsed_from_stamina_restoration += Time.fixedDeltaTime;

                if (time_elapsed_from_stamina_restoration >= time_to_start_restoring_stamina) can_restore_stamina = true;
            }

            if (can_restore_stamina) // Restore Stamina
            {
                if (!can_run) stamina += stamina_restored_per_frame_when_fatigued;
                else stamina += stamina_restored_per_frame;

                stamina = Mathf.Clamp(stamina, 0, max_stamina);
                GI.hud.stamina_bar.value = stamina;

                if (!can_run && stamina >= min_stamina_to_run_when_fatigued) can_run = true;
            }
        }
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        float dt = Time.deltaTime;

        { // Overdose
            if (overdose > 0f)
            {
                if (is_overdosed) { overdose -= Time.deltaTime * overdose_reduced_per_frame_when_overdosed; if (overdose <= 0f) is_overdosed = false; }
                else overdose -= Time.deltaTime * overdose_reduced_per_frame;

                overdose = Mathf.Clamp(overdose, 0f, max_overdose);
                GI.hud.overdose_bar.value = overdose;
            }
        }

        // @TODO: turn off aim when switching a gun, and prevents player from aiming for a short period of time (for animations)
        { // Aiming
            if (Input.GetKeyDown(KeyCode.Mouse1) && GI.player_inventory.is_equiped_with_a_gun())
            {
                is_aiming = !is_aiming;
                GI.hud.gun_reticle.SetActive(!is_aiming);
                fov_percentage = fov_percentage < 1f ? 1f - fov_percentage : 0f;
                if (is_aiming)
                {
                    GI.fp_controller.mouseLook.XSensitivity = GI.player_inventory.get_equiped_weapon().gun.aiming_sensitivity.x;
                    GI.fp_controller.mouseLook.YSensitivity = GI.player_inventory.get_equiped_weapon().gun.aiming_sensitivity.y;
                }
                else
                {
                    GI.fp_controller.mouseLook.XSensitivity = start_mouse_sensitivity.x;
                    GI.fp_controller.mouseLook.YSensitivity = start_mouse_sensitivity.y;
                }
            }

            // Increases / Decreases FOV over time.
            if (fov_percentage < 1f)
            {
                Gun equiped_gun = GI.player_inventory.get_equiped_weapon().gun;
                float from = is_aiming ? start_fov : equiped_gun.fov_when_aiming;
                float to = is_aiming ? equiped_gun.fov_when_aiming : start_fov;
                
                fov_percentage += dt * equiped_gun.fov_speed;
                fov_percentage = Mathf.Clamp(fov_percentage, 0f, 1f);
                GI.fp_camera.fieldOfView = Mathf.Lerp(from, to, fov_percentage);

                lerp_flashlight_range(fov_percentage);
                lerp_flashlight_spot_angle(fov_percentage);

                equiped_gun.lerp_aim_position(fov_percentage);
                equiped_gun.lerp_aim_rotation(fov_percentage);
            }
        }

        { // Flashlight
            if (Input.GetKeyDown(KeyCode.L))
            {
                flashlight.gameObject.SetActive(!flashlight.gameObject.activeSelf);
            }
        }

        //Camera.main.transform.rotation = Quaternion.Euler(Input.GetAxis("Vertical") * 5f, 0f, -Input.GetAxis("Horizontal") * 5f);

        { // Head Bob
            if (GI.fp_controller.Moving || head_bob_speed > 0f)
            {
                float tau = 2 * Mathf.PI;
                float x = tau * head_bob_speed + 0f;
                float y = amplitude * Mathf.Sin(x); // Mathf.Sin() receives the angle in radians.
                Camera.main.transform.localPosition = camera_start_position + Vector3.up * y;

                head_bob_speed += dt * frequency;
                if (GI.fp_controller.Moving && head_bob_speed >= .5f) head_bob_speed -= .5f; // Resets the time so it not trepass the float limit.
                else if (head_bob_speed > .5f) head_bob_speed = 0f; // Stabilization. Makes the camera return to its start position

                // asin(2*PI * t * f + 0)
                // y = a * sin(2*PI * t * f + 0)
                // y = amplitude * Mathf.Sin(2*Mathf.PI * Time.time * frequency + 0);
                // "Time.time * frequency" == "head_bob_speed += dt * frequency" every frame.
                // On Time == 1, the wave completes a cicle; So 0.5f is half a cicle.
            }
        }

        { // Weapon Bob
            if (current_weapon_bob_location >= weapon_bob_radius || current_weapon_bob_location <= 0f) weapon_bob_direction *= -weapon_bob_direction; // change the direction when get on the limits

            current_weapon_bob_location += weapon_bob_direction;
            current_weapon_bob_location = Mathf.Clamp(current_weapon_bob_location, 0f, weapon_bob_radius);

            float y = Mathf.Sqrt(Mathf.Pow(weapon_bob_radius, 2) - Mathf.Pow(current_weapon_bob_location, 2));
            weapon_holster.transform.localPosition = new Vector2(current_weapon_bob_location, y);
        }
    }

    public void lerp_flashlight_range(float fov_percentage)
    {
        float from = is_aiming ? base_flashlight_range : aiming_flashlight_range;
        float to = is_aiming ? aiming_flashlight_range : base_flashlight_range;

        flashlight.range = Mathf.Lerp(from, to, fov_percentage);
    }

    public void lerp_flashlight_spot_angle(float fov_percentage)
    {
        float from = is_aiming ? base_flashlight_spot_angle : aiming_flashlight_spot_angle;
        float to = is_aiming ? aiming_flashlight_spot_angle : base_flashlight_spot_angle;

        flashlight.spotAngle = Mathf.Lerp(from, to, fov_percentage);
    }

    public void change_health_amount(float value)
    {
        if (!is_alive) return;
        if (value >= Mathf.Epsilon && health >= max_health) return;

        if (value > 0) Debug.Log($"Health restored by {value} points!");
        health += value;
        health = Mathf.Clamp(health, 0f, max_health);
        GI.hud.health_bar.value = health;

        if (health < Mathf.Epsilon)
        {
            // Game Over;
            is_alive = false;
            Time.timeScale = 0f;
        }
    }

    public void change_stamina_amount(float value)
    {
        Debug.Log($"Stamina restored by {value} points!");
        stamina += value;
        stamina = Mathf.Clamp(stamina, 0, max_stamina);
        GI.hud.stamina_bar.value = stamina;

        if (value < 0f && stamina <= 0f) can_run = false;
        else if (value > 0f) can_run = true; // Player will be able to run again if he gains stamina through this method.
    }

    public void change_terror_amount(float value)
    {
        Debug.Log($"Terror modified by {value} points");
        terror += value;
        terror = Mathf.Clamp(terror, 0, max_terror);
        GI.hud.terror_bar.value = terror;

        // Change the terror level based on the current terror
        if (terror >= terror_level_3) { current_terror_level = 3; GI.fp_controller.movementSettings.RunMultiplier = run_multiplier_on_terror_level_3; } // Terrified
        else if (terror >= terror_level_2) { current_terror_level = 2; GI.fp_controller.movementSettings.RunMultiplier = run_multiplier_on_terror_level_2; } // Afraid
        else if (terror >= terror_level_1) { current_terror_level = 1; GI.fp_controller.movementSettings.RunMultiplier = run_multiplier_on_terror_level_1; } // Scared
        else { current_terror_level = 0; GI.fp_controller.movementSettings.RunMultiplier = base_run_multiplier; } // No terror - Normal
    }

    public void change_overdose_amount(float value)
    {
        Debug.Log($"Overdose changed by {value} amount");
        overdose += value;

        if (!is_overdosed && overdose >= max_overdose) // Apply overdose
        {
            Debug.Log("Overdose enabled");
            is_overdosed = true;
            //overdose_t = time_to_apply_overdose_debuff;
        }
        else if (is_overdosed && overdose <= 0) // Stops overdose
        {
            Debug.Log("Overdose disabled");
            is_overdosed = false;
        }

        overdose = Mathf.Clamp(overdose, 0, max_overdose);
        GI.hud.overdose_bar.value = overdose;
    }

    // @TODO: change this name to "use_consumable", "use_pills" or something like that
    public void use_item(ItemType item)
    {
        switch (item)
        {
            // @TODO: add a message to player saying that his status are already full... Or just disable "use button"
            case ItemType.HEALTH_PILL:
                {
                    if (health >= max_health)
                    {
                        Debug.Log("Health already full");
                        return;
                    }

                    if (is_overdosed) change_overdose_amount(GI.items_in_game.overdose_added_when_consuming_a_pill_while_overdosed);
                    else change_overdose_amount(GI.items_in_game.base_overdose_added_when_consuming_a_pill);

                    if (is_overdosed) change_health_amount(GI.items_in_game.base_health_restored_by_health_pill * GI.items_in_game.pill_efficiency_when_overdosed);
                    else change_health_amount(GI.items_in_game.base_health_restored_by_health_pill);
                }
                break;

            case ItemType.STAMINA_PILL:
                {
                    if (stamina >= max_stamina)
                    {
                        Debug.Log("Stamina already full");
                        return;
                    }

                    if (is_overdosed) change_overdose_amount(GI.items_in_game.overdose_added_when_consuming_a_pill_while_overdosed);
                    else change_overdose_amount(GI.items_in_game.base_overdose_added_when_consuming_a_pill);

                    if (is_overdosed) change_stamina_amount(GI.items_in_game.base_stamina_restored_by_stamina_pill * GI.items_in_game.pill_efficiency_when_overdosed);
                    else change_stamina_amount(GI.items_in_game.base_stamina_restored_by_stamina_pill);

                }
                break;

            case ItemType.ANTI_TERROR_PILL:
                {
                    if (terror <= 0f)
                    {
                        Debug.Log("Terror already empty");
                        return;
                    }

                    if (is_overdosed) change_overdose_amount(GI.items_in_game.overdose_added_when_consuming_a_pill_while_overdosed);
                    else change_overdose_amount(GI.items_in_game.base_overdose_added_when_consuming_a_pill);

                    if (is_overdosed) change_terror_amount(-(GI.items_in_game.base_terror_reduced_by_anti_terror_pill * GI.items_in_game.pill_efficiency_when_overdosed));
                    else change_terror_amount(-GI.items_in_game.base_terror_reduced_by_anti_terror_pill);
                }
                break;

            case ItemType.ANTI_OVERDOSE_PILL:
                {
                    if (overdose <= 0f)
                    {
                        Debug.Log("Overdose already empty");
                    }
                    change_overdose_amount(-GI.items_in_game.base_overdose_reduced_by_anti_overdose_pill);
                }
                break;

            default:
                Debug.Assert(false);
                break;
        }
    }
}
