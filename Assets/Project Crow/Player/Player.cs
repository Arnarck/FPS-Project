using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Health and Stamina
    bool is_alive = true, can_run = true, can_restore_stamina;
    float current_health, current_stamina;
    float current_max_health, current_max_stamina;
    float time_elapsed_from_stamina_restoration;

    [Header("Health")]
    [SerializeField] Slider healthBar;
    [SerializeField] [Range(0f, 100f)] float startHealth = 50f;
    [SerializeField] [Range(0f, 100f)] float maxHealth = 100f;

    [Header("Stamina")]
    [SerializeField] Slider staminaBar;
    [SerializeField] [Range(0f, 100f)] float startStamina = 50f;
    [SerializeField] [Range(0f, 100f)] float maxStamina = 100f;
    [Tooltip("The stamina amount the player must recover to run again, if he runs out of stamina.")]
    [SerializeField] [Range(0f, 1f)] float min_stamina_to_run = 10f;
    [SerializeField] float stamina_filled_per_frame = .5f;
    [SerializeField] float stamina_consumed_per_frame = .5f;
    [SerializeField] float time_to_start_stamina_restoration = 1f;

    public bool CanRun { get => can_run; }
    public bool IsAlive { get => is_alive; }
    public float MaxHealth { get => maxHealth; }
    public float MaxStamina { get => maxStamina; }

    void Awake()
    {
        GI.player = this;
        GI.fp_camera = Camera.main;
    }

    void Start()
    {
        // Health and Stamina
        current_max_health = maxHealth;
        current_max_stamina = maxStamina;

        current_health = startHealth;
        healthBar.value = startHealth;
        healthBar.maxValue = maxHealth;

        current_stamina = startStamina;
        staminaBar.value = current_stamina;
        staminaBar.maxValue = maxStamina;
    }

    private void FixedUpdate()
    {
        handle_stamina_process();
    }

    void handle_stamina_process()
    {
        if (GI.fp_controller.Running) // Consume stamina
        {
            time_elapsed_from_stamina_restoration = 0f; // Resets the counter
            can_restore_stamina = false;

            current_stamina -= stamina_consumed_per_frame;
            current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
            staminaBar.value = current_stamina;

            if (current_stamina < Mathf.Epsilon) can_run = false;
        }
        else if (!can_restore_stamina) // Increases the counter
        {
            time_elapsed_from_stamina_restoration += Time.fixedDeltaTime;

            if (time_elapsed_from_stamina_restoration >= time_to_start_stamina_restoration) can_restore_stamina = true;
        }


        if (can_restore_stamina) // Restore Stamina
        {
            current_stamina += stamina_filled_per_frame * GI.thirst.restoration_effectiveness;
            current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
            staminaBar.value = current_stamina;

            if (!can_run && current_stamina >= current_max_stamina * min_stamina_to_run) can_run = true;
        }
    }

    public void clamp_max_health(float value)
    {
        value = Mathf.Clamp(value, 0f, maxHealth);
        current_max_health = value;

        if (current_health > value)
        {
            current_health = value;
            healthBar.value = value;
        }
    }

    public void clamp_max_stamina(float value)
    {
        value = Mathf.Clamp(value, 0f, maxStamina);
        current_max_stamina = value;

        if (current_stamina > value)
        {
            current_stamina = value;
            staminaBar.value = value;
            can_run = true;
        }
    }

    public void change_health_amount(float value)
    {
        if (!is_alive || current_health >= current_max_health) return;

        current_health += value;
        current_health = Mathf.Clamp(current_health, 0f, current_max_health);
        healthBar.value = current_health;

        if (current_health < Mathf.Epsilon)
        {
            // Game Over;
            is_alive = false;
            Time.timeScale = 0f;
        }
    }
}
