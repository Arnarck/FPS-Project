using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    AudioSource _audioSource;
    Gun[] avaliable_guns = new Gun[4];
    int current_gun;

    [SerializeField] AudioClip switchSound;
    [Range(0, 9)] [SerializeField] int initialGun = 0;

    void Awake()
    {
        GI.gun_switcher = this;
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Add avaliable guns to list.
        for (int i = 0; i < avaliable_guns.Length; i++)
        {
            avaliable_guns[i] = transform.GetChild(i).GetComponent<Gun>();
        }

        initialGun = Mathf.Clamp(initialGun, 0, avaliable_guns.Length - 1);
        switch_gun(initialGun);
    }

    public void switch_gun(int desiredWeapon)
    {
        if (desiredWeapon < 0 || desiredWeapon >= avaliable_guns.Length) return;

        for (int i = 0; i < avaliable_guns.Length; i++)
        {
            bool isDesiredGun = (i == desiredWeapon ? true : false);
            avaliable_guns[i].gameObject.SetActive(isDesiredGun);

            // Updates active gun.
            if (isDesiredGun)
            {
                current_gun = i;
                _audioSource.PlayOneShot(switchSound);
            }
        }
    }

    void Update()
    {
        { // Switch Input
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                switch_gun(current_gun + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                switch_gun(current_gun - 1);
            }
        }
    }

    public Gun get_current_gun()
    {
        return avaliable_guns[current_gun];
    }
}
