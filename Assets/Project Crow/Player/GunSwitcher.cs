using System.Collections.Generic;
using UnityEngine;

public class GunSwitcher : MonoBehaviour
{
    AudioSource audio_source;
    Gun[] avaliable_guns = new Gun[4];
    Knife knife;
    int current_gun;

    [SerializeField] AudioClip switch_sound;

    void Awake()
    {
        GI.gun_switcher = this;
        audio_source = GetComponent<AudioSource>();
    }

    void Start()
    {
        // Add avaliable guns to list.
        for (int i = 0; i < avaliable_guns.Length; i++)
        {
            // TODO Add a validation for knife
            avaliable_guns[i] = transform.GetChild(i).GetComponent<Gun>();
        }
        knife = transform.GetChild(avaliable_guns.Length).GetComponent<Knife>();

        audio_source.PlayOneShot(switch_sound);
        avaliable_guns[current_gun].gameObject.SetActive(true);
    }

    // TODO Think about better code for this part
    public void switch_gun(int direction)
    {
        //if (current_gun + direction >= avaliable_guns.Length || current_gun + direction < 0) return;
        if (current_gun + direction > avaliable_guns.Length || current_gun + direction < 0) return; // Knife condition

        if (direction > 0)
        {
            for (int i = current_gun + 1; i < avaliable_guns.Length; i++)
            {
                // Updates active gun.
                if (avaliable_guns[i].is_collected)
                {
                    avaliable_guns[current_gun].gameObject.SetActive(false); // Disables the current gun
                    current_gun = i;
                    audio_source.PlayOneShot(switch_sound);
                    avaliable_guns[i].gameObject.SetActive(true); // Enables the new gun
                    return;
                }
            }

            // Knife condition
            avaliable_guns[current_gun].gameObject.SetActive(false);
            current_gun = avaliable_guns.Length;
            knife.gameObject.SetActive(true);
            audio_source.PlayOneShot(switch_sound);
            GI.player_inventory.is_knife_equiped = true;
        }
        else
        {
            if (current_gun == 4) GI.player_inventory.is_knife_equiped = false; // Knife Condition

            for (int i = current_gun - 1; i >= 0; i--)
            {
                // Updates active gun.
                if (avaliable_guns[i].is_collected)
                {
                    // Knife condition
                    if (current_gun != 4) avaliable_guns[current_gun].gameObject.SetActive(false);
                    else knife.gameObject.SetActive(false);

                    current_gun = i;
                    audio_source.PlayOneShot(switch_sound);
                    avaliable_guns[i].gameObject.SetActive(true);
                    return;
                }
            }
        }
    }

    void Update()
    {
        { // Switch Input
            if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            {
                switch_gun(-1);
            }
            else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
            {
                switch_gun(1);
            }
        }
    }

    public void collect_gun(int i)
    {
        avaliable_guns[i].is_collected = true;
    }

    public Gun get_current_gun()
    {
        return avaliable_guns[current_gun];
    }
}
