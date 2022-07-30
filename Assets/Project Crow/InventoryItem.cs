using UnityEngine;

public enum ItemType
{
    NONE,

    // Weapons
    KNIFE,
    PISTOL,
    SHOTGUN,
    ASSAULT_RIFLE,
    SUBMACHINE_GUN,

    // Ammo Type
    PISTOL_AMMO,
    SHOTGUN_AMMO,
    ASSAULT_RIFLE_AMMO,
    SUBMACHINE_GUN_AMMO,

    // Consumables
    HEALTH_PILL,
    STAMINA_PILL,
    FLASHLIGHT_BATTERY,

    // Important

    COUNT
}

public class InventoryItem
{
    public string m_name;
    public int stored_amount;
    public bool is_avaliable = false;
    public ItemType type;
    public SlotData ui;
}
