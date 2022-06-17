using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    int _currentGun;

    public GunSwitcher Instance { get; private set; }

    AudioSource _audioSource;
    List<Transform> _avaliableGuns = new List<Transform>();

    [SerializeField] AudioClip switchSound;
    [Range(0, 9)] [SerializeField] int initialGun = 0;

    void Awake()
    {
        Instance = this;
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        AddAvaliableGuns();

        initialGun = Mathf.Clamp(initialGun, 0, _avaliableGuns.Count - 1);
        SwitchGun(initialGun);
    }

    void AddAvaliableGuns()
    {
        foreach (Transform weapon in transform)
        {
            _avaliableGuns.Add(weapon);
        }
    }

    public void SwitchGun(int desiredWeapon)
    {
        if (desiredWeapon < 0 || desiredWeapon >= _avaliableGuns.Count) return;

        for (int i = 0; i < _avaliableGuns.Count; i++)
        {
            bool isDesiredGun = (i == desiredWeapon ? true : false);
            _avaliableGuns[i].gameObject.SetActive(isDesiredGun);

            if (isDesiredGun)
            {
                UpdateActiveGun(i);
            }
        }
    }

    void UpdateActiveGun(int index)
    {
        _currentGun = index;
        _audioSource.PlayOneShot(switchSound);
    }

    void Update()
    {
        ProcessSwitchInput();
    }

    void ProcessSwitchInput()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            SwitchGun(_currentGun + 1);
        }
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            SwitchGun(_currentGun - 1);
        }
    }
}
