using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Coroutine _waitForStaminaRefill, _refillStamina;
    bool _isAlive = true, _canRun = true;
    float _currentHealth, _currentStamina;

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

    void Start()
    {
        _currentHealth = startHealth;
        healthBar.value = startHealth;
        healthBar.maxValue = maxHealth;

        _currentStamina = startStamina;
        staminaBar.value = _currentStamina;
        staminaBar.maxValue = maxStamina;
    }

    public void ModifyHealthAmount(float value)
    {
        if (!_isAlive || _currentHealth >= maxHealth) return;

        _currentHealth += value;
        _currentHealth = Mathf.Clamp(_currentHealth, 0f, maxHealth);
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
        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        staminaBar.value = _currentStamina;

        if (_refillStamina != null) StopCoroutine(_refillStamina);
        
        if (_currentStamina < Mathf.Epsilon) _canRun = false;
    }

    public void IncreaseStamina(float value)
    {
        if (_currentStamina >= maxStamina) return;

        value = Mathf.Abs(value);
        _currentStamina += value;
        _currentStamina = Mathf.Clamp(_currentStamina, 0, maxStamina);
        staminaBar.value = _currentStamina;

        if (!_canRun && _currentStamina >= minStaminaToRun) _canRun = true;
    }

    public void InitializeStaminaRefill()
    {
        if (_waitForStaminaRefill != null) StopCoroutine(_waitForStaminaRefill);

        _waitForStaminaRefill = StartCoroutine(WaitForStaminaRefill());
    }

    // Wait some time until starts filling the stamina bar.
    IEnumerator WaitForStaminaRefill()
    {
        float timeElapsed = 0f;
        while (timeElapsed < timeToRefillStamina)
        {
            timeElapsed += Time.deltaTime;
            yield return Utilities.Instance.waitForEndOfFrame;
        }
        
        _refillStamina = StartCoroutine(RefillStamina());
    }

    // Increases the stamina over time.
    IEnumerator RefillStamina()
    {
        while (_currentStamina < maxStamina)
        {
            IncreaseStamina(staminaFilledPerFrame);
            yield return Utilities.Instance.waitForEndOfFrame;
        }
    }
}
