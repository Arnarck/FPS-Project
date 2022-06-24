using UnityEngine;
using TMPro;

public class AmmoDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammo_in_clip_text;
    [SerializeField] TextMeshProUGUI ammo_in_holster_text;

    void Awake()
    {
        GI.ammo_display = this;
    }

    public void display_ammo_in_clip(int ammoAmount)
    {
        if (ammo_in_clip_text == null) return;
        ammo_in_clip_text.text = ammoAmount.ToString();
    }

    public void display_ammo_in_holster(int ammoAmount)
    {
        if (ammo_in_holster_text == null) return;
        ammo_in_holster_text.text = ammoAmount.ToString();
    }
}
