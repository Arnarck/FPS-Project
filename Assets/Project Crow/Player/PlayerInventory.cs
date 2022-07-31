using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    InventoryItem[] inventory; // Total Inventory capacity;

    public int slots_expanded_after_collecting_expansion_item;
    public int total_capacity;
    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;
    public GameObject slot_prefab;
    public InventoryData inventory_data;

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

    public bool store_item(Item item)
    {
        if (is_cumulative_item(item.type)) return false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable && inventory[i].type.Equals(ItemType.NONE)) // Tries to find an empty and avaliable inventory slot
            {
                set_slot_data(i, inventory_data.item[(int)item.type].item_name, item.type, 1, true, inventory_data.item[(int)item.type].sprite);
                Debug.Log($"Slot {i} filled with {inventory[i].type}!");
                Destroy(item.gameObject);

                return true;
            }
        }

        Debug.LogError("No inventory slots avaliable!");
        return false;
    }

    void set_slot_data(int i, string name, ItemType type, int stored_amount, bool has_item_stored, Sprite sprite)
    {
        //inventory[i].m_name = name;
        inventory[i].type = type;
        inventory[i].stored_amount = stored_amount;
        inventory[i].ui.slot_index = i;
        inventory[i].ui.button.gameObject.SetActive(has_item_stored);
        inventory[i].ui.count_text.text = is_cumulative_item(type) ? stored_amount.ToString() : null;
        inventory[i].ui.button.image.sprite = sprite;

        Debug.Log($"Added {inventory[i].type} to slot {i} and amount {inventory[i].stored_amount}");
    }

    public void store_cumulative_item(Item item)
    {
        int type = (int)item.type;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable)
            {
                if (inventory[i].type.Equals(ItemType.NONE)) // Add item to an empty slot
                {
                    if (inventory_data.item[type].max_capacity >= item.amount) // avaliable space on inventory is greater than item amount
                    {
                        set_slot_data(i, inventory_data.item[type].item_name, item.type, item.amount, true, inventory_data.item[type].sprite);
                        Destroy(item.gameObject);
                        return;
                    }
                    else // item amount is greater than inventory space
                    {
                        int remaining_amount_on_item = item.amount - inventory_data.item[type].max_capacity;
                        set_slot_data(i, inventory_data.item[type].item_name, item.type, inventory_data.item[type].max_capacity, true, inventory_data.item[type].sprite);
                        item.amount = remaining_amount_on_item;
                    }
                }
                else if (inventory[i].type.Equals(item.type) && inventory[i].stored_amount < inventory_data.item[type].max_capacity) // Add item to an existing slot
                {
                    int avaliable_space = inventory_data.item[type].max_capacity - inventory[i].stored_amount;

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
                        inventory[i].stored_amount = inventory_data.item[type].max_capacity;
                        inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
                        item.amount = remaining_amount_on_item;
                    }
                }
            }
        }

        Debug.LogError("No inventory slots avaliable!");
    }

    public void remove_item(int i)
    {

        if (!inventory[i].is_avaliable || inventory[i].type.Equals(ItemType.NONE) || inventory[i].stored_amount == 0)
        {
            Debug.LogError("Trying to remove item from an invalid inventory slot!");
            return;
        }

        Debug.Log($"Removed 1 {inventory[i].type} from slot {i}.");

        inventory[i].stored_amount--;

        if (inventory[i].stored_amount < 1) // Reset inventory slot
        {
            set_slot_data(i, null, ItemType.NONE, 0, false, null);
            return;
        }
        
        // Should only get here with cumulative items
        inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
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
