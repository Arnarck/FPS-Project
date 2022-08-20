using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    float time_elapsed_from_stamina_restoration, overdose_t;
    public bool is_alive = true, can_run = true, can_restore_stamina, is_overdosed;

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

    [Header("Terror")]
    public Slider terror_bar;
    public float terror;
    public float max_terror;
    public float terror_reduced_per_frame;
    public float recoil_multiplier_based_on_terror;

    [Header("Medicine Overdose")]
    public Slider overdose_bar;
    public float overdose;
    public float max_overdose;
    public float time_to_apply_overdose_debuff = 2f;
    public float health_damage = -5f;
    public float terror_applied = 5f;

    void Awake()
    {
        GI.player = this;
        GI.fp_camera = Camera.main;
    }

    void Start()
    {
        health_bar.maxValue = max_health;
        health_bar.value = health;

        stamina_bar.maxValue = max_stamina;
        stamina_bar.value = stamina;

        terror_bar.maxValue = max_terror;
        terror_bar.value = terror;

        overdose_bar.maxValue = max_overdose;
        overdose_bar.value = overdose;
    }

    void FixedUpdate()
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

    void Update()
    {
        // @Arnarck make terror reduce each 3 seconds or something instead of every frame
        { // Terror decay over time
            terror -= Time.deltaTime * terror_reduced_per_frame;
            terror = Mathf.Clamp(terror, 0, max_terror);
            terror_bar.value = terror;

            if (terror >= 85f) recoil_multiplier_based_on_terror = 4f;
            else if (terror >= 60f) recoil_multiplier_based_on_terror = 3f;
            else if (terror >= 30f) recoil_multiplier_based_on_terror = 2f;
            else recoil_multiplier_based_on_terror = 1f;
        }

        { // Overdose
            if (is_overdosed)
            {
                overdose_t -= Time.deltaTime;
                if (overdose_t <= 0f) // Apply overdose debuff and restarts the cronometer
                {
                    overdose_t += time_to_apply_overdose_debuff;
                    change_health_amount(health_damage);
                    change_terror_amount(terror_applied);
                    change_overdose_amount(-15f);
                }
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

    public void change_terror_amount(float value)
    {
        Debug.Log($"Terror modified by {value} points");
        terror += value;
        terror = Mathf.Clamp(terror, 0, max_terror);
        terror_bar.value = terror;
    }

    public void change_overdose_amount(float value)
    {
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
        overdose_bar.value = overdose;
    }
}
