using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    InventoryItem[] inventory; // Total Inventory capacity;

    public int slots_expanded_after_collecting_expansion_item;
    public int total_capacity;
    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;
    public GameObject slot_prefab;

    void Awake()
    {
        GI.player_inventory = this;
        inventory = new InventoryItem[total_capacity];

        for (int i = 0; i < inventory.Length; i++) // Creates the inventory, and disable all slots
        {
            inventory[i] = new InventoryItem();
            inventory[i].ui = Instantiate(slot_prefab).GetComponent<SlotData>();
            inventory[i].ui.slot_index = i;
            inventory[i].ui.gameObject.SetActive(false);
            inventory[i].ui.transform.SetParent(ui_inventory_handler.transform);
        }
    }

    void Update()
    {
        { // Toggle inventory ui
            if (Input.GetKeyDown(KeyCode.I))
            {
                ui_inventory_screen.SetActive(!ui_inventory_screen.activeInHierarchy);
            }   
        }
    }

    // TODO Create an class for cumulative items (like ammo), make this class and Item class inherit from "Collectable" class.
    // Collectable will only have an name and Sprite.
    // Use Collectable class for items like "InventoryExpansion" or "Ammo Holster Expansion".
    // Use Collectable class in "ItemPickup.cs" to get the name and image of an item
    public bool store_item(Item item)
    {
        if (is_cumulative_item(item.type)) return false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable && inventory[i].type.Equals(ItemType.NONE)) // Tries to find an empty and avaliable inventory slot
            {
                set_slot_data(i, item.m_name, item.type, 1, true, item.sprite);

                //inventory[i].m_name = item.m_name;
                //inventory[i].type = item.type;
                //inventory[i].stored_amount = 1;
                //inventory[i].ui.button.gameObject.SetActive(true);
                //inventory[i].ui.count_text.text = null; // Don't display item count for single items ("single" means 1 item per slot)
                //inventory[i].ui.button.image.sprite = item.sprite;

                Debug.Log($"Slot {i} filled with {inventory[i].type}!");

                return true;
            }
        }

        Debug.LogError("No inventory slots avaliable!");
        return false;
    }

    void set_slot_data(int i, string name, ItemType type, int stored_amount, bool has_item_stored, Sprite sprite)
    {
        inventory[i].m_name = name;
        inventory[i].type = type;
        inventory[i].stored_amount = stored_amount;
        inventory[i].ui.slot_index = i;
        inventory[i].ui.button.gameObject.SetActive(has_item_stored);
        inventory[i].ui.count_text.text = is_cumulative_item(type) ? stored_amount.ToString() : null;
        inventory[i].ui.button.image.sprite = sprite;

        Debug.Log($"Added {inventory[i].type} to slot {i} and amount {inventory[i].stored_amount}");
    }

    public void store_cumulative_item(CumulativeItem item)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable)
            {
                if (inventory[i].type.Equals(ItemType.NONE)) // Add item to an empty slot
                {
                    if (get_max_capacity(item.type) >= item.amount) // avaliable space on inventory is greater than item amount
                    {
                        set_slot_data(i, item.m_name, item.type, item.amount, true, item.sprite);
                        Destroy(item.gameObject);
                        return;
                    }
                    else // item amount is greater than inventory space
                    {
                        int remaining_amount_on_item = item.amount - get_max_capacity(item.type);
                        set_slot_data(i, item.m_name, item.type, get_max_capacity(item.type), true, item.sprite);
                        item.amount = remaining_amount_on_item;
                    }
                }
                else if (inventory[i].type.Equals(item.type) && inventory[i].stored_amount < get_max_capacity(item.type)) // Add item to an existing slot
                {
                    int avaliable_space = get_max_capacity(item.type) - inventory[i].stored_amount;

                    if (avaliable_space >= item.amount) // avaliable space on inventory is greater than item amount
                    {
                        inventory[i].stored_amount += item.amount;
                        inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
                        item.amount = 0;
                        Destroy(item.gameObject);
                        return;
                    }
                    else // item amount is greater than inventory space
                    {
                        int remaining_amount_on_item = item.amount - avaliable_space;
                        inventory[i].stored_amount = get_max_capacity(item.type);
                        inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
                        item.amount = remaining_amount_on_item;
                    }
                }
            }
        }

        Debug.LogError("No inventory slots avaliable!");
    }

    public void remove_cumulative_item(CumulativeItem item)
    {

    }

    public void remove_item(int i)
    {

        if (!inventory[i].is_avaliable || inventory[i].type.Equals(ItemType.NONE) || inventory[i].stored_amount == 0)
        {
            Debug.LogError("Trying to remove item from an invalid inventory slot!");
            return;
        }

        Debug.Log($"Removed 1 {inventory[i].type} from slot {i}.");

        set_slot_data(i, null, ItemType.NONE, 0, false, null);
        //inventory[i].m_name = null;
        //inventory[i].type = ItemType.NONE;
        //inventory[i].stored_amount = 0;
        //inventory[i].ui.button.gameObject.SetActive(false);
        //inventory[i].ui.count_text.text = null;
        //inventory[i].ui.button.image.sprite = null;
    }

    public void expand_inventory_capacity()
    {
        int amount_to_expand = slots_expanded_after_collecting_expansion_item;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].is_avaliable)
            {
                amount_to_expand--;
                inventory[i].is_avaliable = true;
                inventory[i].type = ItemType.NONE;
                inventory[i].ui.gameObject.SetActive(true);
                inventory[i].ui.button.gameObject.SetActive(false);

                if (amount_to_expand < 1) return;
            }
        }

        Debug.LogError("All inventory slots unlocked. Couldn't expand inventory by " + amount_to_expand + " slots.");
    }

    public int get_max_capacity(ItemType type)
    {
        int i = (int)type + 1;

        if (type.Equals(ItemType.NONE))
        {
            Debug.LogError("Trying to get the max capacity of NONE");
            return 0;
        }

        if (i >= 6 && i <= 9) return 10;

        return 1;
    }

    public bool can_expand_capacity()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].is_avaliable) return true;
        }

        return false;
    }

    public bool is_cumulative_item(ItemType item)
    {
        if ((int)item >= 6 && (int)item <= 9) return true; // Ammo
        if ((item.Equals(ItemType.FLASHLIGHT_BATTERY))) return true;

        return false;
    }
}
