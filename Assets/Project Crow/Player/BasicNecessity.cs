using UnityEngine;
using UnityEngine.UI;

enum NecessityType
{
    Hunger,
    Thirst
}

public class BasicNecessity : MonoBehaviour
{
    [HideInInspector]
    public float restoration_effectiveness;
    float current_value, time_elapsed_since_last_decay;
    public bool has_value = true;

    [SerializeField] NecessityType type;
    [SerializeField] Slider necessity_bar;
    [SerializeField] float time_to_decay = 10f;
    [SerializeField] float start_value = 100f;
    [SerializeField] float max_value = 100f;
    [SerializeField] float value_reduced_for_each_decay = 5f;
    [SerializeField] [Range(0f, 1f)] float max_health_or_stamina_when_bar_is_empty;

    void Awake()
    {
        if (type.Equals(NecessityType.Hunger)) GI.hunger = this;
        else GI.thirst = this;
    }

    void Start()
    {
        current_value = start_value;
        necessity_bar.value = start_value;
        necessity_bar.maxValue = max_value;
        calculate_item_restoration_effectiveness();
        try_to_clamp_max_health_or_stamina_based_on_necessity_type();
    }

    void Update()
    {
        try_to_reduce_current_value();
    }

    void try_to_reduce_current_value()
    {
        if (!has_value) return;

        if (time_elapsed_since_last_decay >= time_to_decay)
        {
            time_elapsed_since_last_decay = 0f;
            reduce_necessity_amount();
        }
        else
        {
            time_elapsed_since_last_decay += Time.deltaTime;
        }
    }

    void calculate_item_restoration_effectiveness()
    {
        if (current_value >= 75f) restoration_effectiveness = 1.5f;
        else if (current_value >= 40f) restoration_effectiveness = 1f;
        else if (current_value >= 20f) restoration_effectiveness = 0.75f;
        else restoration_effectiveness = 0.5f;
    }

    void try_to_clamp_max_health_or_stamina_based_on_necessity_type()
    {
        // Limits the max health / stamina if _currentValue is 0. If not, sets the base value.
        if (current_value < Mathf.Epsilon)
        {
            if (type.Equals(NecessityType.Hunger)) GI.player.clamp_max_health(GI.player.MaxHealth * max_health_or_stamina_when_bar_is_empty);
            //else GI.player.clamp_max_stamina(GI.player.MaxStamina * max_health_or_stamina_when_bar_is_empty);
        }
        else
        {
            if (type.Equals(NecessityType.Hunger)) GI.player.clamp_max_health(GI.player.MaxHealth);
            //else GI.player.clamp_max_stamina(GI.player.MaxStamina);
        }
    }

    void reduce_necessity_amount()
    {
        current_value -= value_reduced_for_each_decay;
        current_value = Mathf.Clamp(current_value, 0f, max_value);
        necessity_bar.value = current_value;

        if (current_value < Mathf.Epsilon) has_value = false;

        calculate_item_restoration_effectiveness();
        try_to_clamp_max_health_or_stamina_based_on_necessity_type();
    }

    public void increase_value(float value)
    {
        has_value = true;
        current_value += value;
        current_value = Mathf.Clamp(current_value, 0f, max_value);
        necessity_bar.value = current_value;

        calculate_item_restoration_effectiveness();
        try_to_clamp_max_health_or_stamina_based_on_necessity_type();
    }
}
