﻿using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Health and Stamina
    public bool is_alive = true, can_run = true, can_restore_stamina;
    public float current_health, current_stamina;
    public float current_max_health, current_max_stamina;
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
    [SerializeField] float stamina_restored_per_frame = .5f;
    [SerializeField] float stamina_consumed_per_frame = .5f;
    [SerializeField] float time_to_start_stamina_restoration = 1f;

    public bool CanRun { get => can_run; }

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
        if (GI.pause_game.game_paused) return;

        { // Stamina processes
            if (GI.fp_controller.Running) // Consume stamina
            {
                time_elapsed_from_stamina_restoration = 0f; // Resets the counter
                can_restore_stamina = false;

                current_stamina -= stamina_consumed_per_frame;
                current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
                staminaBar.value = current_stamina;

                if (current_stamina < Mathf.Epsilon) can_run = false;
            }
            else if (!can_restore_stamina) // Cooldown to restore stamina
            {
                time_elapsed_from_stamina_restoration += Time.fixedDeltaTime;

                if (time_elapsed_from_stamina_restoration >= time_to_start_stamina_restoration) can_restore_stamina = true;
            }


            if (can_restore_stamina) // Restore Stamina
            {
                current_stamina += stamina_restored_per_frame;
                current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
                staminaBar.value = current_stamina;

                if (!can_run && current_stamina >= current_max_stamina * min_stamina_to_run) can_run = true;
            }
        }
    }

    public void change_health_amount(float value)
    {
        if (!is_alive) return;
        if (value >= Mathf.Epsilon && current_health >= current_max_health) return;

        if (value > 0) Debug.Log($"Health restored by {value} points!");
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

    public void increase_stamina(float value)
    {
        Debug.Log($"Stamina restored by {value} points!");
        current_stamina += value;
        current_stamina = Mathf.Clamp(current_stamina, 0, current_max_stamina);
        staminaBar.value = current_stamina;
        can_run = true;
    }
}
