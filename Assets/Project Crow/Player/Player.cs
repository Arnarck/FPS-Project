using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Coroutine _refillStamina;
    bool _isAlive = true, _canRun = true;
    float _currentHealth, _currentStamina;
    float _currentMaxHealth, _currentMaxStamina;

    [Header("Health")]
    [SerializeField] Slider healthBar;
    [SerializeField] [Range(0f, 100f)] float startHealth = 50f;
    [SerializeField] [Range(0f, 100f)] float maxHealth = 100f;
    [Header("Stamina")]
    [SerializeField] Slider staminaBar;
    [SerializeField] [Range(0f, 100f)] float startStamina = 50f;
    [SerializeField] [Range(0f, 100f)] float maxStamina = 100f;
    [Tooltip("The stamina amount the player must recover to run again, if he runs out of stamina.")]
    [SerializeField] [Range(0f, 100f)] float minStaminaToRun = 10f;
    [SerializeField] [Range(0f, 1f)] float staminaFilledPerFrame = .5f;
    [SerializeField] float timeToRefillStamina = 1f;

    public bool CanRun { get => _canRun; }
    public bool IsAlive { get => _isAlive; }
    public float MaxHealth { get => maxHealth; }
    public float MaxStamina { get => maxStamina; }

    void Start()
    {
        _currentMaxHealth = maxHealth;
        _currentMaxStamina = maxStamina;

        _currentHealth = startHealth;
        healthBar.value = startHealth;
        healthBar.maxValue = maxHealth;

        _currentStamina = startStamina;
        staminaBar.value = _currentStamina;
        staminaBar.maxValue = maxStamina;
    }

    public void ClampMaxHealth(float value)
    {
        value = Mathf.Clamp(value, 0f, maxHealth);
        _currentMaxHealth = value;

        if (_currentHealth > value)
        {
            _currentHealth = value;
            healthBar.value = value;
        }
    }

    public void ClampMaxStamina(float value)
    {
        value = Mathf.Clamp(value, 0f, maxStamina);
        _currentMaxStamina = value;

        if (_currentStamina > value)
        {
            _currentStamina = value;
            staminaBar.value = value;
        }
    }

    public void ModifyHealthAmount(float value)
    {
        if (!_isAlive || _currentHealth >= _currentMaxHealth) return;

        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, _currentMaxHealth);
        healthBar.value = _currentHealth;

        if (_currentHealth < Mathf.Epsilon)
        {
            // Game Over;
            _isAlive = false;
            Time.timeScale = 0f;
        }
    }

    public void DecreaseStamina(float value)
    {
        if (!_canRun) return;

        _currentStamina -= value;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _currentMaxStamina);
        staminaBar.value = _currentStamina;

        if (_refillStamina != null) StopCoroutine(_refillStamina);
        
        if (_currentStamina < Mathf.Epsilon) _canRun = false;
    }

    public void IncreaseStamina(float value)
    {
        if (_currentStamina >= _currentMaxStamina) return;

        value = Mathf.Abs(value);
        _currentStamina += value;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, _currentMaxStamina);
        staminaBar.value = _currentStamina;

        if (!_canRun && _currentStamina >= minStaminaToRun) _canRun = true;
    }

    public void InitializeStaminaRefill()
    {
        if (_refillStamina != null) StopCoroutine(_refillStamina);

        _refillStamina = StartCoroutine(RefillStamina());
    }

    IEnumerator RefillStamina()
    {
        // Cooldown until refill the stamina.
        float timeElapsed = 0f;
        while (timeElapsed < timeToRefillStamina)
        {
            timeElapsed += Time.deltaTime;
            yield return Util.Instance.waitForEndOfFrame;
        }

        // Refill stamina over the time.
        while (_currentStamina < maxStamina)
        {
            Debug.Log(staminaFilledPerFrame * GI.Instance.thirst.RecoverMultiplier);
            IncreaseStamina(staminaFilledPerFrame * GI.Instance.thirst.RecoverMultiplier);
            yield return Util.Instance.waitForEndOfFrame;
        }
    }
}
