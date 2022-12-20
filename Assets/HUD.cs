using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI ammo_in_clip, ammo_in_holster, pickup_text;
    public Slider health_bar, stamina_bar, terror_bar, overdose_bar;
    public GameObject gun_reticle, ammo_display, inventory_screen, inventory_handler;
    public ItemMenu item_menu;
    public Image pickup_image;
    public RectTransform pickup_interface, canvas, crosshair_interface;
    public GameObject debug_image;
    public CanvasScaler canvas_scaler;

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

    public void display_gun_crosshair(float crosshair_radius_on_screen)
    {
        Vector2 crosshair_size;
        crosshair_size.x = canvas_scaler.referenceResolution.x * crosshair_radius_on_screen; // "radius" is in percentage (eg: 5% of the Screen Size)
        crosshair_size.y = canvas_scaler.referenceResolution.x * crosshair_radius_on_screen; // Using "X" instead of "Y" to keep the crosshair with a circular format. If we use "Y" instead, the crosshair will have a oval format.

        crosshair_interface.sizeDelta = crosshair_size * 2f; // "*2" because the crosshair is anchored at the center of the screen, so it's length is half of the screen size.
    }

    public void display_crosshair_of_equiped_weapon()
    {
        bool is_equiped_with_a_gun = GI.player_inventory.is_equiped_with_a_gun();

        gun_reticle.SetActive(!is_equiped_with_a_gun);
        crosshair_interface.gameObject.SetActive(is_equiped_with_a_gun);
    }
}
