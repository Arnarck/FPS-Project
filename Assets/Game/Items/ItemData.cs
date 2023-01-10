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
    //[Header("Health Pill")]
    //public float base_overdose_added_by_consuming_health_pill = 30f;
    //[Header("Stamina Pill")]
    //public float base_overdose_added_by_consuming_stamina_pill = 30f;
    //[Header("Anti Terror Pill")]
    //public float base_overdose_added_by_consuming_anti_terror_pill = 30f;
   // [Header("Anti Overdose Pill")]
    public float gun_integrity_restored_by_repair_kit = 100f;
   [Header("Pill Buffs")]
    public float base_health_restored_by_health_pill = 50f;
    public float base_stamina_restored_by_stamina_pill = 100f;
    public float base_terror_reduced_by_anti_terror_pill = 100f;
    public float base_overdose_reduced_by_anti_overdose_pill = 100f;
    [Header("Pill Debuffs")]
    public float pill_efficiency_when_overdosed = .5f; // Multiplier
    public float base_overdose_added_when_consuming_a_pill = 30f;
    public float overdose_added_when_consuming_a_pill_while_overdosed = 45f;
    [Header("Items in Game")]
    public ItemDetails[] items;

    private void Awake()
    {
        GI.items_in_game = this;
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
