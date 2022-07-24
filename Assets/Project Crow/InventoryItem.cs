using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int amount;
    public int max_amount;
    public bool is_locked = true;
    public Sprite m_image;
    public ItemType type;
}
