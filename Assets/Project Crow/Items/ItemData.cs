using System.Collections;
using System.Collections.Generic;
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
    ANTI_TERROR_PILL,
    ANTI_OVERDOSE_PILL,

    // Tools
    GUN_REPAIR_KIT,
    INVENTORY_EXPANSION,
    FLASHLIGHT_BATTERY,

    // Important

    COUNT
}

[System.Serializable]
public struct ItemDetails
{
    public string item_name;
    //public string description;
    //public bool is_consumable;
    //public bool is_stockable; // can stock more than 1 item amount per slot.
    public ItemType type;
    public Sprite sprite;
}


public class ItemData : MonoBehaviour
{
    public ItemDetails[] items;

    private void Awake()
    {
        GI.item_data = this;
    }

    public ItemDetails get_details_of(ItemType type)
    {
        foreach (ItemDetails item in items)
        {
            if (item.type == type) return item;
        }

        Debug.LogError($"The item {type} was not found!");
        return default;
    }

    [ContextMenu("Generate One Item Of Each Type")]
    public void generate_one_item_of_each_type()
    {
        int size = System.Enum.GetNames(typeof(ItemType)).Length;
        items = new ItemDetails[size - 2]; // "-2" to exclued NONE and COUNT

        // 0 = NONE
        // The for loop excludes NONE and COUNT
        for (int i = 1; i < (int)ItemType.COUNT; i++) items[i - 1].type = (ItemType)i;
    }
}
