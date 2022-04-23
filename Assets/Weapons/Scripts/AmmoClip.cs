using UnityEngine;
using System.Collections;

public class AmmoClip : MonoBehaviour
{
    int _currentAmmo;
    bool _isReloading, _hasAmmo, _hasStarted;

    public bool HasAmmo { get => _hasAmmo; }
    public bool IsReloading { get => _isReloading; }

    [SerializeField] float reloadTime = 1f;

    [Header("Ammo Settings")]
    [SerializeField] AmmoType ammoType;
    [SerializeField] int maxAmount = 30;
    [SerializeField] int initialAmount = 30;

    void OnEnable()
    {
        if (_hasStarted)
        {
            AmmoDisplay.Instance.UpdateAmmoInClip(_currentAmmo);
            AmmoDisplay.Instance.UpdateAmmoInHolster(AmmoHolster.Instance.GetCurrentAmmo(ammoType));
        }
    }

    void OnDisable()
    {
        if (_isReloading)
        {
            // Cancel reload process so the gun won't reenable in reloading state.
            _isReloading = false;
        }
    }

    void Start()
    {
        _hasStarted = true;
        _hasAmmo = _currentAmmo > 0 ? true : false;
        _currentAmmo = Mathf.Clamp(initialAmount, 0, maxAmount);

        AmmoDisplay.Instance.UpdateAmmoInClip(_currentAmmo);
        AmmoDisplay.Instance.UpdateAmmoInHolster(AmmoHolster.Instance.GetCurrentAmmo(ammoType));
    }

    public void ReduceAmmo()
    {
        _currentAmmo--;
        if (_currentAmmo < 1)
        {
            _currentAmmo = 0;
            _hasAmmo = false;
        }
        AmmoDisplay.Instance.UpdateAmmoInClip(_currentAmmo);
    }

    public void Reload()
    {
        StartCoroutine(CoolDownToReload());
    }

    IEnumerator CoolDownToReload()
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
        int ammoInHolster = AmmoHolster.Instance.GetCurrentAmmo(ammoType);

        if (ammoInHolster >= spentAmmo)
        {
            _currentAmmo = maxAmount;
            AmmoHolster.Instance.ReduceAmmo(ammoType, spentAmmo);
        }
        else
        {
            _currentAmmo += ammoInHolster;
            AmmoHolster.Instance.ReduceAmmo(ammoType, ammoInHolster);
        }

        AmmoDisplay.Instance.UpdateAmmoInClip(_currentAmmo);
    }
}
