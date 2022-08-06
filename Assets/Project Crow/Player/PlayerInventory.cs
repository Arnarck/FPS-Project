using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [HideInInspector] public InventoryItem[] inventory; // Total Inventory capacity;
    [HideInInspector] public int current_slot_selected_on_item_menu, previous_slot_selected_on_item_menu = -1;

    public int slots_expanded_after_collecting_expansion_item;
    public int total_capacity;
    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;
    public ItemMenu ui_item_menu;
    public GameObject slot_prefab;

    void Awake()
    {
        GI.player_inventory = this;
        inventory = new InventoryItem[total_capacity];

        for (int i = 0; i < inventory.Length; i++) // Creates the inventory, and disable all slots
        {
            inventory[i] = new InventoryItem();
            inventory[i].ui = Instantiate(slot_prefab).GetComponent<InventorySlotUI>();
            inventory[i].ui.index = i;
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
                if (ui_inventory_screen.activeInHierarchy == false)
                {
                    ui_item_menu.gameObject.SetActive(false);
                    previous_slot_selected_on_item_menu = -1;
                }
            }   
        }
    }

    public bool store_item(Item item)
    {
        if (is_cumulative(item.type)) return false;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable && inventory[i].type.Equals(ItemType.NONE)) // Tries to find an empty and avaliable inventory slot
            {
                set_slot_data(i, item.type, 1, true, item.sprite);
                Debug.Log($"Slot {i} filled with {inventory[i].type}!");
                Destroy(item.gameObject);

                return true;
            }
        }

        Debug.LogError("No inventory slots avaliable!");
        return false;
    }

    void set_slot_data(int i, ItemType type, int stored_amount, bool has_item_stored, Sprite sprite)
    {
        inventory[i].type = type;
        inventory[i].stored_amount = stored_amount;
        inventory[i].ui.index = i;
        inventory[i].ui.button.gameObject.SetActive(has_item_stored);
        inventory[i].ui.count_text.text = is_cumulative(type) ? stored_amount.ToString() : null;
        inventory[i].ui.button.image.sprite = sprite;

        Debug.Log($"Added {inventory[i].type} to slot {i} and amount {inventory[i].stored_amount}");
    }

    public void store_cumulative_item(Item item)
    {
        if (!is_cumulative(item.type)) return;

        int type = (int)item.type;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_avaliable)
            {
                if (inventory[i].type.Equals(ItemType.NONE)) // Add item to an empty slot
                {
                    if (InventoryData.max_capacity[type] >= item.amount) // avaliable space on inventory is greater than item amount
                    {
                        set_slot_data(i, item.type, item.amount, true, item.sprite);
                        Destroy(item.gameObject);
                        return;
                    }
                    else // item amount is greater than inventory space
                    {
                        int remaining_amount_on_item = item.amount - InventoryData.max_capacity[type];
                        set_slot_data(i, item.type, InventoryData.max_capacity[type], true, item.sprite);
                        item.amount = remaining_amount_on_item;
                    }
                }
                else if (inventory[i].type.Equals(item.type) && inventory[i].stored_amount < InventoryData.max_capacity[type]) // Add item to an existing slot
                {
                    int avaliable_space = InventoryData.max_capacity[type] - inventory[i].stored_amount;

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
                        inventory[i].stored_amount = InventoryData.max_capacity[type];
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
            set_slot_data(i, ItemType.NONE, 0, false, null);
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

    public bool is_cumulative(ItemType item)
    {
        if ((int)item >= 6 && (int)item <= 9) return true; // Ammo
        if ((item.Equals(ItemType.FLASHLIGHT_BATTERY))) return true;

        return false;
    }

    public bool is_weapon(ItemType item)
    {
        if ((int)item >= 1 && (int)item <= 5) return true;
        return false;
    }

    public bool is_consumable(ItemType item)
    {
        if ((int)item >= 10 && (int)item <= 12) return true;
        return false;
    }

    // TODO Add functionallity of equiping a gun.
    // if player don't have any gun, auto equip the first one he finds.
    // Create 4 slots that will work as shortcuts for weapons?

    // TODO (Later) Change "item menu" position based on which slot activated it
    public void toggle_item_menu(int i)
    {
        ItemType item = inventory[i].type;
        current_slot_selected_on_item_menu = i;

        if (current_slot_selected_on_item_menu == previous_slot_selected_on_item_menu)
        {
            ui_item_menu.gameObject.SetActive(false);
            previous_slot_selected_on_item_menu = -1;
            return;
        }
        else
        {
            if (is_weapon(item)) // Weapons
            {
                Debug.Log("WEAPON selected");
                toggle_item_menu_options(false, true, false);
            }
            else if (is_cumulative(item)) // Ammo (or cumulative items)
            {
                Debug.Log("CUMULATIVE ITEM selected");
                toggle_item_menu_options(false, false, true);
            }
            else if (is_consumable(item)) // Consumable items
            {
                Debug.Log("CONSUMABLE ITEM selected");
                toggle_item_menu_options(true, false, false);
            }

            ui_item_menu.activate_item_menu();
        }

        previous_slot_selected_on_item_menu = current_slot_selected_on_item_menu;
    }

    public void toggle_item_menu_options(bool option1_active, bool option2_active, bool option3_active)
    {
        ui_item_menu.options[0].SetActive(option1_active);
        ui_item_menu.options[1].SetActive(option2_active);
        ui_item_menu.options[2].SetActive(option3_active);
    }
}
