using System.Collections;
using UnityEngine;
using UnityEngine.UI;

enum NecessityType
{
    Hunger,
    Thirst
}

public class Player : MonoBehaviour
{
    // Health and Stamina
    bool is_alive = true, can_run = true, can_restore_stamina;
    float current_health, current_stamina;
    float current_max_health, current_max_stamina;
    float time_elapsed_from_stamina_restoration;

    // Food
    WaitForSeconds wait_for_time_to_food_decay;
    float current_food_amount, health_percentage_restored_by_medkits;
    bool is_out_of_food;

    // Water
    WaitForSeconds wait_for_time_to_water_decay;
    float current_water_amount, stamina_percentage_restored_over_time;
    bool is_out_of_water;


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

    [Header("Hunger")]
    [SerializeField] Slider hunger_bar;
    [SerializeField] float max_food_amount = 100f;
    [SerializeField] float start_food_amount = 100f;
    [SerializeField] float time_to_food_decay = 10f;
    [SerializeField] float food_reduced_for_each_decay = 5f;
    [Tooltip("Limit the max health / stamina")]
    [SerializeField] [Range(0f, 1f)] float health_reduced_when_out_of_food; // Multiplier

    [Header("Thirst")]
    [SerializeField] Slider thirst_bar;
    [SerializeField] float max_water_amount = 100f;
    [SerializeField] float start_water_amount = 100f;
    [SerializeField] float time_to_water_decay = 10f;
    [SerializeField] float water_reduced_for_each_decay = 5f;
    [SerializeField] [Range(0f, 1f)] float stamina_reduced_when_out_of_water; // Multiplier

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

        // Food
        wait_for_time_to_food_decay = new WaitForSeconds(time_to_food_decay);
        current_food_amount = start_food_amount;
        hunger_bar.value = start_food_amount;
        hunger_bar.maxValue = max_food_amount;
        health_percentage_restored_by_medkits = calculate_item_restoration_effectivity(NecessityType.Hunger, current_food_amount);
    }

    IEnumerator reduce_food_over_time()
    {
        while (!is_out_of_food)
        {
            yield return wait_for_time_to_food_decay;
            current_food_amount -= food_reduced_for_each_decay;
            current_food_amount = Mathf.Clamp(current_food_amount, 0f, max_food_amount);
            hunger_bar.value = current_food_amount;

            if (current_food_amount < Mathf.Epsilon) is_out_of_food = true;

            health_percentage_restored_by_medkits = calculate_item_restoration_effectivity(NecessityType.Hunger, current_food_amount);
        }
    }

    void increase_food_amount(float amount)
    {
        is_out_of_food = false;
        current_food_amount += amount;
        current_food_amount = Mathf.Clamp(current_food_amount, 0f, max_food_amount);
        hunger_bar.value = current_food_amount;
    }

    float calculate_item_restoration_effectivity(NecessityType type, float current_amount)
    {
        float recover_multiplier = 1f;

        if (current_amount >= 75f) recover_multiplier = 1.25f;
        else if (current_amount >= 40f) recover_multiplier = 1f;
        else if (current_amount >= 20f) recover_multiplier = 0.75f;
        else recover_multiplier = 0.5f;

        // Clamps max health / stamina when out of food / water
        if (type == NecessityType.Hunger)
        {
            if (is_out_of_food) clamp_max_health(MaxHealth * health_reduced_when_out_of_food);
            else clamp_max_health(MaxHealth);
        }
        else
        {
            if (is_out_of_water) clamp_max_stamina(MaxStamina * stamina_reduced_when_out_of_water);
            else clamp_max_stamina(MaxStamina);
        }

        return recover_multiplier;
    }

    private void FixedUpdate()
    {
        { // Process stamina consumed and refilled
            if (GI.fp_controller.Running)
            {
                time_elapsed_from_stamina_restoration = 0f; // Resets the counter
                can_restore_stamina = false;

                current_stamina -= stamina_consumed_per_frame;
                current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
                staminaBar.value = current_stamina;

                if (current_stamina < Mathf.Epsilon) can_run = false;
            }
            else if (!can_restore_stamina)
            {
                time_elapsed_from_stamina_restoration += Time.fixedDeltaTime;

                if (time_elapsed_from_stamina_restoration >= time_to_start_stamina_restoration) can_restore_stamina = true;
            }
        
        
            if (can_restore_stamina)
            {
                current_stamina += stamina_filled_per_frame * GI.thirst.RecoverMultiplier;
                current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
                staminaBar.value = current_stamina;

                if (!can_run && current_stamina >= current_max_stamina * min_stamina_to_run) can_run = true;
            }
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
