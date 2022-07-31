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

public class InventoryData : MonoBehaviour
{
    [System.Serializable]
    public struct ItemData
    {
        public string item_name;
        public string description;
        public int max_capacity;
        public Sprite sprite;
    }

    public ItemData[] item;
}
