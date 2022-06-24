using UnityEngine;
using System.Collections;

public class AmmoClip : MonoBehaviour
{

    int _currentAmmo;
    bool _isReloading, _hasAmmo, _hasStarted;

    [SerializeField] float reloadTime = 1f;

    [Header("Ammo Settings")]
    public AmmoType ammo_type;
    [SerializeField] int maxAmount = 30;
    [SerializeField] int initialAmount = 30;

    public bool HasAmmo { get => _hasAmmo; }
    public bool IsReloading { get => _isReloading; }

    void OnEnable()
    {
        if (_hasStarted)
        {
            GI.ammo_display.display_ammo_in_clip(_currentAmmo);
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
        _currentAmmo = Mathf.Clamp(initialAmount, 0, maxAmount); // Clamps the initial ammo amount.
        _hasAmmo = _currentAmmo > 0 ? true : false;

        GI.ammo_display.display_ammo_in_clip(_currentAmmo);
        GI.ammo_display.display_ammo_in_holster(GI.ammo_holster.GetCurrentAmmo(ammo_type));
    }

    public void ReduceAmmo()
    {
        _currentAmmo--;
        if (_currentAmmo < 1)
        {
            _currentAmmo = 0;
            _hasAmmo = false;
        }
        GI.ammo_display.display_ammo_in_clip(_currentAmmo);
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
        int spentAmmo = maxAmount - _currentAmmo;
        int ammoInHolster = GI.ammo_holster.GetCurrentAmmo(ammo_type);

        if (ammoInHolster >= spentAmmo)
        {
            _currentAmmo = maxAmount;
            GI.ammo_holster.ReduceAmmo(ammo_type, spentAmmo);
        }
        else
        {
            _currentAmmo += ammoInHolster;
            GI.ammo_holster.ReduceAmmo(ammo_type, ammoInHolster);
        }

        GI.ammo_display.display_ammo_in_clip(_currentAmmo);
    }
}
