using UnityEngine;
using UnityEngine.UI;

public abstract class BasicNecessity : MonoBehaviour
{
    float _currentValue, _recoverMultiplier, _timeElapsed;
    bool _hasValue = true;

    [SerializeField] Slider bar;
    [SerializeField] float timeToDecay = 10f;
    [SerializeField] [Range(0f, 100f)] float startValue = 100f;
    [SerializeField] [Range(0f, 100f)] float maxValue = 100f;
    [SerializeField] [Range(0f, 10f)] float decayValue = 5f;

    public float CurrentValue { get => _currentValue; }
    public float RecoverMultiplier { get => _recoverMultiplier; }

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
            DecayValue();
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
        else if (_currentValue >= 20f) _recoverMultiplier = .75f;
        else _recoverMultiplier = .5f;
    }

    void DecayValue()
    {
        _currentValue -= decayValue;
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
