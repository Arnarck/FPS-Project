using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    AudioSource _audioSource;
    List<Gun> m_avaliable_guns = new List<Gun>();
    int m_current_gun;

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
        foreach (Transform child in transform)
        {
            m_avaliable_guns.Add(child.GetComponent<Gun>());
        }

        initialGun = Mathf.Clamp(initialGun, 0, m_avaliable_guns.Count - 1);
        switch_gun(initialGun);
    }

    public void switch_gun(int desiredWeapon)
    {
        if (desiredWeapon < 0 || desiredWeapon >= m_avaliable_guns.Count) return;

        for (int i = 0; i < m_avaliable_guns.Count; i++)
        {
            bool isDesiredGun = (i == desiredWeapon ? true : false);
            m_avaliable_guns[i].gameObject.SetActive(isDesiredGun);

            // Updates active gun.
            if (isDesiredGun)
            {
                m_current_gun = i;
                _audioSource.PlayOneShot(switchSound);
            }
        }
    }

    void Update()
    {
        { // Switch Input
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                switch_gun(m_current_gun + 1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                switch_gun(m_current_gun - 1);
            }
        }
    }

    public Gun get_current_gun()
    {
        return m_avaliable_guns[m_current_gun];
    }

    // OPTIMIZE THIS
    // Maybe use a single script for all gun funcionality
    public AmmoType get_current_ammo_type()
    {
        return m_avaliable_guns[m_current_gun].GetComponent<AmmoClip>().ammo_type;
    }
}
