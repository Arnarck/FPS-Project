using UnityEngine;

[RequireComponent(typeof(Gun))]
[RequireComponent(typeof(AmmoClip))]
public class GunControls : MonoBehaviour
{
    Gun _gun;
    AmmoClip _clip;

    void Awake()
    {
        _gun = GetComponent<Gun>();
        _clip = GetComponent<AmmoClip>();

        if (transform.parent != null)
        {
            if (transform.GetComponentInParent<GunSwitcher>() == null)
            {
                transform.parent.gameObject.AddComponent<GunSwitcher>();
            }
        }
        else
        {
            Debug.LogError("Gun Controls script must contain a parent Game Object!");
        }
    }

    void Update()
    {
        ProcessFireInput();
        ProcessReloadInput();
    }

    void ProcessFireInput()
    {
        if (!_gun.CanShoot || _clip.IsReloading || !_clip.HasAmmo) return;

        if (_gun.IsAuto)
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                StartShootProcess();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                StartShootProcess();
            }
        }
    }

    void StartShootProcess()
    {
        _gun.Shoot();
        _clip.ReduceAmmo();
    }

    void ProcessReloadInput()
    {
        if (_clip.IsReloading) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            _clip.Reload();
        }
    }
}
