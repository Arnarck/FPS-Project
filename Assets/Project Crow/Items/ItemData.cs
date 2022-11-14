using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public ItemDetails get_item(ItemType type)
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
