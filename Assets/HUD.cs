using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI ammo_in_clip, ammo_in_holster;
    public Slider health_bar, stamina_bar, terror_bar, overdose_bar;
    public GameObject gun_reticle, ammo_display;

    void Awake()
    {
        GI.hud = this;
    }

    public void display_ammo_in_clip(int ammo_amount)
    {
        if (ammo_in_clip == null) return;
        ammo_in_clip.text = ammo_amount.ToString();
    }

    public void display_ammo_in_holster(int ammo_amount)
    {
        if (ammo_in_holster == null) return;
        ammo_in_holster.text = ammo_amount.ToString();
    }
}
