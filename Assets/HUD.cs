using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public GameObject gun_reticle;
    public GameObject debug_screen;
    public Slider health_bar;
    public Slider stamina_bar;
    public Slider terror_bar;
    public Slider overdose_bar;

    void Awake()
    {
        GI.hud = this;
    }

    void Update()
    {

    }
}
