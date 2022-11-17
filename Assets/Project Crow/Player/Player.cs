using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float start_fov;
    [HideInInspector] public float fov_percentage = 1f;
    float time_elapsed_from_stamina_restoration, overdose_t, terror_t;
    public bool is_alive = true, can_run = true, can_restore_stamina, is_overdosed;
    [HideInInspector] Vector2 start_mouse_sensitivity;

    [Header("Health")]
    public float health = 50;
    public float max_health = 100f;

    [Header("Stamina")]
    public float stamina = 50;
    public float max_stamina = 100f;
    public float min_stamina_to_run = 10f;
    public float stamina_restored_per_frame = .5f;
    public float stamina_consumed_per_frame = .5f;
    public float time_to_start_restoring_stamina = 1f;

    [Header("Terror")]
    public float terror;
    public float max_terror;
    public float terror_reduced_per_tick;
    public float recoil_multiplier_based_on_terror = 1f;
    public float time_to_reduce_terror = 3f;

    [Header("Medicine Overdose")]
    public float overdose;
    public float max_overdose;
    public float time_to_apply_overdose_debuff = 2f;
    public float overdose_reduced_per_tick = -15f;
    public float health_damage = -5f;
    public float terror_applied = 5f;
    public float stamina_restored_when_overdosed = .5f; // Multiplier

    [Header("Aim")]
    public bool is_aiming;
    //public float fov_when_aiming = 50f;
    public float fov_speed = 10f;

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

        start_mouse_sensitivity.x = GI.fp_controller.mouseLook.XSensitivity;
        start_mouse_sensitivity.y = GI.fp_controller.mouseLook.YSensitivity;

        GI.hud.ammo_display.SetActive(GI.player_inventory.is_equiped_with_a_gun());
        
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
                // Restore less stamina when overdosed
                if (is_overdosed) stamina += stamina_restored_per_frame * stamina_restored_when_overdosed;
                else stamina += stamina_restored_per_frame;

                stamina = Mathf.Clamp(stamina, 0, max_stamina);
                GI.hud.stamina_bar.value = stamina;

                if (!can_run && stamina >= max_stamina * min_stamina_to_run) can_run = true;
            }
        }

    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Terror
            if (terror > 0f)
            {
                terror_t -= Time.deltaTime;
                if (terror_t <= 0f)
                {
                    terror_t += time_to_reduce_terror;
                    change_terror_amount(terror_reduced_per_tick);
                }
            }
        }

        { // Overdose
            if (overdose > 0f)
            {
                overdose_t -= Time.deltaTime;
                if (overdose_t <= 0f) // Apply overdose debuff and restarts the cronometer
                {
                    overdose_t += time_to_apply_overdose_debuff;
                    change_overdose_amount(overdose_reduced_per_tick);
                    if (is_overdosed)
                    {
                        change_health_amount(health_damage);
                        change_terror_amount(terror_applied);
                    }
                }
            }
        }

        // @Arnarck turn off aim when switching a gun, and prevents player from aiming for a short period of time (for animations)
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

            if (fov_percentage < 1f)
            {
                Gun equiped_gun = GI.player_inventory.get_equiped_weapon().gun;
                float from = is_aiming ? start_fov : equiped_gun.fov_when_aiming;
                float to = is_aiming ? equiped_gun.fov_when_aiming : start_fov;
                
                fov_percentage += Time.deltaTime * equiped_gun.fov_speed;
                fov_percentage = Mathf.Clamp(fov_percentage, 0f, 1f);
                GI.fp_camera.fieldOfView = Mathf.Lerp(from, to, fov_percentage);
                equiped_gun.toggle_aim_position(fov_percentage);
                equiped_gun.toggle_aim_rotation(fov_percentage);
            }
        }
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

    public void increase_stamina(float value)
    {
        Debug.Log($"Stamina restored by {value} points!");
        stamina += value;
        stamina = Mathf.Clamp(stamina, 0, max_stamina);
        GI.hud.stamina_bar.value = stamina;
        can_run = true;
    }

    public void change_terror_amount(float value)
    {
        if (terror <= 0f && value > 0f) terror_t = time_to_reduce_terror;

        Debug.Log($"Terror modified by {value} points");
        terror += value;
        terror = Mathf.Clamp(terror, 0, max_terror);
        GI.hud.terror_bar.value = terror;

        // Change gun recoil based on terror amount
        if (terror >= 85f) recoil_multiplier_based_on_terror = GI.Config.recoil_multiplier_on_terror_level_3;
        else if (terror >= 60f) recoil_multiplier_based_on_terror = GI.Config.recoil_multiplier_on_terror_level_2;
        else if (terror >= 30f) recoil_multiplier_based_on_terror = GI.Config.recoil_multiplier_on_terror_level_1;
        else recoil_multiplier_based_on_terror = 1f; // No terror
    }

    public void change_overdose_amount(float value)
    {
        // Only set the counter the first time the overdose is increased
        if (overdose <= 0f && value > 0f) overdose_t = time_to_apply_overdose_debuff;

        Debug.Log($"Overdose changed by {value} amount");
        overdose += value;

        if (!is_overdosed && overdose >= max_overdose) // Apply overdose
        {
            Debug.Log("Overdose enabled");
            is_overdosed = true;
            overdose_t = time_to_apply_overdose_debuff;
        }
        else if (is_overdosed && overdose <= 0) // Stops overdose
        {
            Debug.Log("Overdose disabled");
            is_overdosed = false;
        }

        overdose = Mathf.Clamp(overdose, 0, max_overdose);
        GI.hud.overdose_bar.value = overdose;
    }

    // @Arnarck change this name to "use_consumable", "use_pills" or something like that
    public void use_item(ItemType item)
    {
        switch (item)
        {
            // @Arnarck add a message to player saying that his status are already full... Or just disable "use button"
            case ItemType.HEALTH_PILL:
                {
                    if (health >= max_health)
                    {
                        Debug.Log("Health already full");
                        return;
                    }
                    change_health_amount(GI.Config.health_restored_by_pill);

                    // Increases overdose amount after consuming a pill.
                    // Increases overdose EVEN MORE if player is overdosed
                    if (is_overdosed) change_overdose_amount(GI.Config.overdose_increased_by_health_pill * GI.Config.overdosed_multiplier);
                    else change_overdose_amount(GI.Config.overdose_increased_by_health_pill);
                }
                break;

            case ItemType.STAMINA_PILL:
                {
                    if (stamina >= max_stamina)
                    {
                        Debug.Log("Stamina already full");
                        return;
                    }
                    increase_stamina(GI.Config.stamina_restored_by_pill);

                    if (is_overdosed) change_overdose_amount(GI.Config.overdose_increased_by_stamina_pill * GI.Config.overdosed_multiplier);
                    else change_overdose_amount(GI.Config.overdose_increased_by_stamina_pill);
                }
                break;

            case ItemType.ANTI_TERROR_PILL:
                {
                    if (terror <= 0f)
                    {
                        Debug.Log("Terror already empty");
                        return;
                    }
                    change_terror_amount(-GI.Config.terror_decreased_by_pill);

                    if (is_overdosed) change_overdose_amount(GI.Config.overdose_increased_by_terror_pill * GI.Config.overdosed_multiplier);
                    else change_overdose_amount(GI.Config.overdose_increased_by_terror_pill);
                }
                break;

            case ItemType.ANTI_OVERDOSE_PILL:
                {
                    if (overdose <= 0f)
                    {
                        Debug.Log("Overdose already empty");
                    }
                    change_overdose_amount(-max_overdose);
                }
                break;

            default:
                Debug.Assert(false);
                break;
        }
    }
}
