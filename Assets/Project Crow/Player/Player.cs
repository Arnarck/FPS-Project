using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    // Health and Stamina
    public bool is_alive = true, can_run = true, can_restore_stamina;
    float time_elapsed_from_stamina_restoration;

    [Header("Health")]
    [SerializeField] Slider health_bar;
    public float health = 50;
    public float max_health = 100f;

    [Header("Stamina")]
    [SerializeField] Slider stamina_bar;
    public float stamina = 50;
    public float max_stamina = 100f;
    public float min_stamina_to_run = 10f;
    public float stamina_restored_per_frame = .5f;
    public float stamina_consumed_per_frame = .5f;
    public float time_to_start_restoring_stamina = 1f;

    public bool CanRun { get => can_run; }

    void Awake()
    {
        GI.player = this;
        GI.fp_camera = Camera.main;
    }

    void Start()
    {
        // Health and Stamina
        health_bar.value = health;
        health_bar.maxValue = max_health;

        stamina_bar.value = stamina;
        stamina_bar.maxValue = max_stamina;
    }

    private void FixedUpdate()
    {
        if (GI.pause_game.game_paused) return;

        { // Stamina processes
            if (GI.fp_controller.Running) // Consume stamina
            {
                time_elapsed_from_stamina_restoration = 0f; // Resets the counter
                can_restore_stamina = false;

                stamina -= stamina_consumed_per_frame;
                stamina = Mathf.Clamp(stamina, 0, max_stamina);
                stamina_bar.value = stamina;

                if (stamina < Mathf.Epsilon) can_run = false;
            }
            else if (!can_restore_stamina) // Cooldown to restore stamina
            {
                time_elapsed_from_stamina_restoration += Time.fixedDeltaTime;

                if (time_elapsed_from_stamina_restoration >= time_to_start_restoring_stamina) can_restore_stamina = true;
            }


            if (can_restore_stamina) // Restore Stamina
            {
                stamina += stamina_restored_per_frame;
                stamina = Mathf.Clamp(stamina, 0, max_stamina);
                stamina_bar.value = stamina;

                if (!can_run && stamina >= max_stamina * min_stamina_to_run) can_run = true;
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
        health_bar.value = health;

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
        stamina_bar.value = stamina;
        can_run = true;
    }
}
