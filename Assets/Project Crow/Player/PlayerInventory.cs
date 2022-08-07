using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [HideInInspector] public InventoryItem[] inventory; // Total Inventory capacity;
    [HideInInspector] public Weapon[] weapons;
    [HideInInspector] public int current_slot_selected_on_item_menu, previous_slot_selected_on_item_menu = -1;
    [HideInInspector] public WeaponType equiped_weapon = WeaponType.NONE;

    public int slots_expanded_after_collecting_expansion_item;
    public int total_capacity;
    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;
    public ItemMenu ui_item_menu;
    public GameObject slot_prefab;
    public Transform weapon_holster;

    void Awake()
    {
        GI.player_inventory = this;
        inventory = new InventoryItem[total_capacity];

        // TODO maybe turn InventoryItem into a struct
        for (int i = 0; i < inventory.Length; i++) // Creates the inventory, and disable all slots
        {
            inventory[i] = new InventoryItem();
            inventory[i].ui = Instantiate(slot_prefab).GetComponent<InventorySlotUI>();
            inventory[i].ui.index = i;
            inventory[i].ui.gameObject.SetActive(false);
            inventory[i].ui.transform.SetParent(ui_inventory_handler.transform, false);
        }

        weapons = new Weapon[weapon_holster.childCount];
        // Disable all weapons at the start of the game
        for (int i = 0; i < weapon_holster.childCount; i++)
        {
            weapons[i] = weapon_holster.GetChild(i).GetComponent<Weapon>();
            weapons[i].gameObject.SetActive(false);
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
                set_slot_data(i, item.type, 1, item.sprite);
                Debug.Log($"Slot {i} filled with {inventory[i].type}!");
                Destroy(item.gameObject);

                return true;
            }
        }

        Debug.LogError("No inventory slots avaliable!");
        return false;
    }

    void set_slot_data(int i, ItemType type, int stored_amount, Sprite sprite)
    {
        inventory[i].type = type;
        inventory[i].stored_amount = stored_amount;
        inventory[i].ui.index = i;
        inventory[i].ui.button.gameObject.SetActive(stored_amount > 0 ? true : false);
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
                        set_slot_data(i, item.type, item.amount, item.sprite);
                        Destroy(item.gameObject);
                        return;
                    }
                    else // item amount is greater than inventory space
                    {
                        int remaining_amount_on_item = item.amount - InventoryData.max_capacity[type];
                        set_slot_data(i, item.type, InventoryData.max_capacity[type], item.sprite);
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
            set_slot_data(i, ItemType.NONE, 0, null);
            return;
        }
        
        // Should only get here with cumulative items
        inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
    }

    public void remove_item(ItemType item, int amount_to_remove)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (item.Equals(inventory[i].type))
            {
                if (inventory[i].stored_amount > amount_to_remove)
                {
                    inventory[i].stored_amount -= amount_to_remove;
                    inventory[i].ui.count_text.text = inventory[i].stored_amount.ToString();
                    return;
                }
                else
                {
                    amount_to_remove -= inventory[i].stored_amount;
                    set_slot_data(i, ItemType.NONE, 0, null);
                    if (amount_to_remove < 1) return;
                }
            }
        }

        Debug.LogError($"Could not remove {amount_to_remove} items!");
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

    public bool is_ammo(ItemType item)
    {
        int current_index = (int)item;
        int first_ammo_index = (int)ItemType.PISTOL_AMMO;
        int last_ammo_index = (int)ItemType.SUBMACHINE_GUN_AMMO;

        return current_index >= first_ammo_index && current_index <= last_ammo_index;
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

    // TODO Disable option of equiping with the weapon is already equiped
    // TODO Create 4 slots to serve as shortcuts?
    public void equip_weapon()
    {
        if (!is_weapon(inventory[current_slot_selected_on_item_menu].type))
        {
            Debug.LogError($"{inventory[current_slot_selected_on_item_menu].type} is not a weapon!");
            return;
        }

        WeaponType weapon = (WeaponType)inventory[current_slot_selected_on_item_menu].type;

        if (weapon.Equals(equiped_weapon))
        {
            Debug.LogError($"{weapon} already equiped!");
            return;
        }
        if (weapon.Equals(WeaponType.NONE) || weapon.Equals(WeaponType.COUNT))
        {
            Debug.LogError($"Trying to equip {weapon} to weapon slot!");
            return;
        }

        // Enables the selected weapon and disable the others
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type.Equals(weapon))
            {
                weapons[i].gameObject.SetActive(true);
                equiped_weapon = weapons[i].type;
            }
            else weapons[i].gameObject.SetActive(false);
        }

        display_total_ammo_of_equiped_weapon();
    }

    public void display_total_ammo_of_equiped_weapon()
    {
        if (equiped_weapon.Equals(WeaponType.KNIFE) || equiped_weapon.Equals(WeaponType.NONE) || equiped_weapon.Equals(WeaponType.COUNT))
        {
            Debug.LogError($"{equiped_weapon} does not have an ammo count");
            return;
        }

        ItemType ammo_type = get_ammo_type_of(equiped_weapon);
        int total_ammo_count = get_total_item_count(ammo_type);
        GI.ammo_display.display_ammo_in_holster(total_ammo_count);
    }

    public ItemType get_ammo_type_of(WeaponType weapon)
    {
        switch (weapon)
        {
            case WeaponType.PISTOL:
                return ItemType.PISTOL_AMMO;

            case WeaponType.SHOTGUN:
                return ItemType.SHOTGUN_AMMO;

            case WeaponType.ASSAULT_RIFLE:
                return ItemType.ASSAULT_RIFLE_AMMO;

            case WeaponType.SUBMACHINE_GUN:
                return ItemType.SUBMACHINE_GUN_AMMO;

            default:
                Debug.Assert(false);
                break;
        }

        Debug.LogError($"{weapon} is a invalid weapon type!");
        return ItemType.NONE;
    }

    public WeaponType get_weapon_type_of(ItemType item)
    {
        switch (item)
        {
            case ItemType.PISTOL:
            case ItemType.PISTOL_AMMO:
                return WeaponType.PISTOL;

            case ItemType.SHOTGUN:
            case ItemType.SHOTGUN_AMMO:
                return WeaponType.SHOTGUN;

            case ItemType.ASSAULT_RIFLE:
            case ItemType.ASSAULT_RIFLE_AMMO:
                return WeaponType.ASSAULT_RIFLE;

            case ItemType.SUBMACHINE_GUN:
            case ItemType.SUBMACHINE_GUN_AMMO:
                return WeaponType.SUBMACHINE_GUN;

            default:
                Debug.Assert(false);
                break;
        }

        Debug.LogError($"{item} is a invalid weapon type!");
        return WeaponType.NONE;
    }

    public int get_total_item_count(ItemType item)
    {
        int item_count = 0;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (item.Equals(inventory[i].type)) item_count += inventory[i].stored_amount;
        }

        return item_count;
    }

    public void reset_slot_data()
    {
        bool result = check_if_item_is_ammo_and_corresponds_to_equiped_weapon(inventory[current_slot_selected_on_item_menu].type);
        set_slot_data(current_slot_selected_on_item_menu, ItemType.NONE, 0, null);
        if (result) display_total_ammo_of_equiped_weapon();
    }

    public bool check_if_item_is_ammo_and_corresponds_to_equiped_weapon(ItemType item)
    {
        if (!is_ammo(item)) return false;
        if (!equiped_weapon.Equals(get_weapon_type_of(item))) return false; // checks if item is an ammo of the equiped weapon

        return true;
    }
}
