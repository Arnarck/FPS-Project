using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    bool _isAlive = true;
    float _currentHealth;

    [SerializeField] Slider healthBar;
    [SerializeField] Slider animatedBar;
    [SerializeField] Slider staminaBar;
    [SerializeField][Range(0f, 100f)] float startHealth = 50f;
    [SerializeField] [Range(0f, 100f)] float maxHealth = 100f;

    public bool IsAlive { get => _isAlive; private set => _isAlive = value; }

    void Start()
    {
        _currentHealth = startHealth;
        healthBar.value = startHealth;
        healthBar.maxValue = maxHealth;
    }

    public void ModifyHealthAmount(int value)
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
}
