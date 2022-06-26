using UnityEngine;
using UnityEngine.UI;

enum NecessityType
{
    Hunger,
    Thirst
}

public class BasicNecessity : MonoBehaviour
{
    float _currentValue, _recoverMultiplier, _timeElapsed;
    bool _hasValue = true;

    [SerializeField] NecessityType type;
    [SerializeField] Slider bar;
    [SerializeField] float timeToDecay = 10f;
    [SerializeField] [Range(0f, 100f)] float startValue = 100f;
    [SerializeField] [Range(0f, 100f)] float maxValue = 100f;
    [SerializeField] [Range(0f, 10f)] float value_reduced = 5f;
    [Tooltip("Limit the max health / stamina")]
    [SerializeField] [Range(0f, 1f)] float statusLimiter;

    public float CurrentValue { get => _currentValue; }
    public float RecoverMultiplier { get => _recoverMultiplier; }

    void Awake()
    {
        if (type.Equals(NecessityType.Hunger)) GI.hunger = this;
        else GI.thirst = this;
    }

    void Start()
    {
        _currentValue = startValue;
        bar.value = startValue;
        bar.maxValue = maxValue;
        CalculateRecoverMultiplier();
    }

    void Update()
    {
        if (_hasValue && _timeElapsed >= timeToDecay)
        {
            _timeElapsed = 0f;
            reduce_necessity_amount();
        }
        else
        {
            _timeElapsed += Time.deltaTime;
        }
    }

    void CalculateRecoverMultiplier()
    {
        if (_currentValue >= 75f) _recoverMultiplier = 1.25f;
        else if (_currentValue >= 40f) _recoverMultiplier = 1f;
        else if (_currentValue >= 20f) _recoverMultiplier = 0.75f;
        else _recoverMultiplier = 0.5f;

        // Limits the max health / stamina if _currentValue is 0. If not, sets the base value.
        if (_currentValue < Mathf.Epsilon)
        {
            if (type.Equals(NecessityType.Hunger)) GI.player.ClampMaxHealth(GI.player.MaxHealth * statusLimiter);
            else GI.player.ClampMaxStamina(GI.player.MaxStamina * statusLimiter);
        }
        else
        {
            if (type.Equals(NecessityType.Hunger)) GI.player.ClampMaxHealth(GI.player.MaxHealth);
            else GI.player.ClampMaxStamina(GI.player.MaxStamina);
        }
    }

    void reduce_necessity_amount()
    {
        _currentValue -= value_reduced;
        _currentValue = Mathf.Clamp(_currentValue, 0f, maxValue);
        bar.value = _currentValue;

        if (_currentValue < Mathf.Epsilon) _hasValue = false;

        CalculateRecoverMultiplier();
    }

    public void IncreaseValue(float value)
    {
        _hasValue = true;
        _currentValue += value;
        _currentValue = Mathf.Clamp(_currentValue, 0f, maxValue);
        bar.value = _currentValue;
    }
}
