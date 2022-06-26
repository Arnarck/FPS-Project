using UnityEngine;
using System.Collections;

public class AmmoClip : MonoBehaviour
{

    int current_amount;
    bool _isReloading, _hasAmmo, _hasStarted;

    [SerializeField] float reloadTime = 1f;

    [Header("Ammo Settings")]
    public AmmoType ammo_type;
    [SerializeField] int max_amount = 30;
    [SerializeField] int initialAmount = 30;

    public bool HasAmmo { get => _hasAmmo; }
    public bool IsReloading { get => _isReloading; }

    void OnEnable()
    {
        if (_hasStarted)
        {
            GI.ammo_display.display_ammo_in_clip(current_amount);
            GI.ammo_display.display_ammo_in_holster(GI.ammo_holster.GetCurrentAmmo(ammo_type));
        }
    }

    void OnDisable()
    {
        // Cancel reload process so the gun won't reenable in reloading state.
        if (_isReloading)
        {
            _isReloading = false;
        }
    }

    void Start()
    {
        _hasStarted = true;
        current_amount = Mathf.Clamp(initialAmount, 0, max_amount); // Clamps the initial ammo amount.
        _hasAmmo = current_amount > 0 ? true : false;

        GI.ammo_display.display_ammo_in_clip(current_amount);
        GI.ammo_display.display_ammo_in_holster(GI.ammo_holster.GetCurrentAmmo(ammo_type));
    }

    public void ReduceAmmo()
    {
        current_amount--;
        if (current_amount < 1)
        {
            current_amount = 0;
            _hasAmmo = false;
        }
        GI.ammo_display.display_ammo_in_clip(current_amount);
    }

    public void Reload()
    {
        StartCoroutine(CooldownToReload());
    }

    // Create an "waitForSeconds" variable when the final reload time be defined.
    IEnumerator CooldownToReload()
    {
        _isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        _isReloading = false;
        _hasAmmo = true;
        ReloadClip();
    }

    void ReloadClip()
    {
        int spent_ammo = max_amount - current_amount;
        int ammo_in_holster = GI.ammo_holster.GetCurrentAmmo(ammo_type);

        if (ammo_in_holster >= spent_ammo)
        {
            current_amount = max_amount;
            GI.ammo_holster.increase_or_reduce(ammo_type, -spent_ammo);
        }
        else
        {
            current_amount += ammo_in_holster;
            GI.ammo_holster.increase_or_reduce(ammo_type, -ammo_in_holster);
        }

        GI.ammo_display.display_ammo_in_clip(current_amount);
    }
}
