using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    InventoryItem[] inventory = new InventoryItem[16]; //Total Inventory capacity;

    public bool is_knife_equiped;
    public GameObject ui_inventory_screen;
    public GameObject ui_inventory_handler;

    void Awake()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            inventory[i] = new InventoryItem();
        }
        GI.player_inventory = this;
        // TODO Max ammo should be added in an static class
        //for (int i = 0; i < inventory.Length; i++)
        //{
        //    inventory[i].is_locked = true;

        //    switch (inventory[i].type)
        //    {
        //        case ItemType.PISTOL_AMMO:
        //            {
        //                inventory[i].max_amount = 75;
        //            }
        //            break;

        //        case ItemType.SHOTGUN_AMMO:
        //            {
        //                inventory[i].max_amount = 16;
        //            }
        //            break;

        //        case ItemType.ASSAULT_RIFLE_AMMO:
        //            {
        //                inventory[i].max_amount = 60;
        //            }
        //            break;

        //        case ItemType.SUBMACHINE_GUN_AMMO:
        //            {
        //                inventory[i].max_amount = 90;
        //            }
        //            break;

        //        default:
        //            Debug.Assert(false);
        //            break;
        //    }
        //}
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ui_inventory_screen.SetActive(!ui_inventory_screen.activeInHierarchy);
            if (ui_inventory_screen.activeInHierarchy)
            {
                for (int i = 0; i < ui_inventory_handler.transform.childCount; i++)
                {
                    ui_inventory_handler.transform.GetChild(i).gameObject.SetActive(!inventory[i].is_locked);
                    if (ui_inventory_handler.transform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        if (inventory[i].type == ItemType.NONE)
                        {
                            ui_inventory_handler.transform.GetChild(i).GetComponent<Image>().color = Color.white;
                        }
                        else
                        {
                            ui_inventory_handler.transform.GetChild(i).GetComponent<Image>().sprite = inventory[i].m_image;
                        }
                    }
                }
            }

        }
    }

    public bool store_or_remove(Consumable item, int amount = 1)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].type.Equals(item.type)) // Add / remove item from an existing slot
            {
                if ((int)item.type >= 6 && (int)item.type <= 9) // Ammo indexes
                {
                    if (inventory[i].amount >= inventory[i].max_amount) continue;
                    if (inventory[i].amount == 0 && amount < 0) continue;

                    inventory[i].amount += amount;
                    inventory[i].amount = Mathf.Clamp(inventory[i].amount, 0, inventory[i].max_amount);
                    return true;
                }
            }
            else if (inventory[i].type.Equals(ItemType.NONE) && amount > 0) // Add item to an new slot
            {
                inventory[i].m_name = item.m_name;
                inventory[i].m_image = item.m_image;
                inventory[i].type = item.type;

                if ((int)item.type >= 6 && (int)item.type <= 9) // Ammo indexes
                {
                    inventory[i].amount = amount;
                }
                else
                {
                    inventory[i].amount = 1;
                }

                return true;
            }
        }

        return false;
    }

    public void expand_inventory_capacity(int amount)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].is_locked && amount > 0)
            {
                amount--;
                inventory[i].is_locked = false;
                inventory[i].type = ItemType.NONE;
            }
        }
    }

    public int get_current_ammo(int index)
    {
        index += 7; // Converts AmmoType to ItemType
        foreach (InventoryItem item in inventory)
        {
            if ((int)item.type == index) return item.amount;
        }

        return 0;
    }
}
