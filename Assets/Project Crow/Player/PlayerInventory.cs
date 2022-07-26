using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    InventoryItem[] inventory; // Total Inventory capacity;

    public int expanded_amount;
    public int total_capacity;
    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;
    public GameObject slot_prefab;

    void Awake()
    {
        inventory = new InventoryItem[total_capacity];
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InventoryItem();
            inventory[i].slot_ui = Instantiate(slot_prefab);
            inventory[i].slot_ui.SetActive(inventory[i].is_avaliable);
            inventory[i].slot_ui.transform.SetParent(ui_inventory_handler.transform);
        }
        GI.player_inventory = this;
    }

    void Update()
    {
        { // Toggle inventory ui
            if (Input.GetKeyDown(KeyCode.I))
            {
                ui_inventory_screen.SetActive(!ui_inventory_screen.activeInHierarchy);
            }   

            if (Input.GetKeyDown(KeyCode.J))
            {
                for (int i =0; i < inventory.Length; i++)
                {
                    Debug.Log($"Index: {i} \nName: {inventory[i].m_name} \n Stored Amount: {inventory[i].stored_amount} \n Type: {inventory[i].type} \n Is Avaliable: {inventory[i].is_avaliable}");
                }
            }
        }
    }

    // TODO Create an class for cumulative items (like ammo), make this class and Item class inherit from "Collectable" class.
    // Collectable will only have an name and Sprite.
    // Use Collectable class for items like "InventoryExpansion" or "Ammo Holster Expansion".
    // Use Collectable class in "ItemPickup.cs" to get the name and image of an item
    public bool store_item(Item item)
    {
        if ((int)item.type >= 6 && (int)item.type <= 9) return false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable && inventory[i].type.Equals(ItemType.NONE)) // Tries to find an empty and avaliable inventory slot
            {
                // TODO Put this data at the slot gameObject... And think on how to get the references for image, name, amount, etc...
                inventory[i].m_name = item.m_name;
                inventory[i].type = item.type;
                inventory[i].stored_amount = 1;
                inventory[i].sprite = item.sprite;

                Debug.Log($"Slot {i} filled with {inventory[i].type}!");

                return true;
            }
        }

        Debug.Log("No inventory slots avaliable!");
        return false;
    }

    public void expand_inventory_capacity()
    {
        int value = expanded_amount;

        if (value < 1) return;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].is_avaliable)
            {
                value--;
                inventory[i].is_avaliable = true;
                inventory[i].type = ItemType.NONE;
                inventory[i].slot_ui.SetActive(true);

                if (value < 1) return;
            }
        }

        Debug.LogError("All inventory slots unlocked. Couldn't expand inventory by " + value + " slots.");
    }

    public bool can_expand_capacity()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].is_avaliable) return true;
        }

        return false;
    }
}
