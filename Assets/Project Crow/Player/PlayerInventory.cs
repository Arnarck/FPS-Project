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
            inventory[i].item = Instantiate(slot_prefab);
            inventory[i].item.SetActive(inventory[i].is_avaliable);
            inventory[i].item.transform.SetParent(ui_inventory_handler.transform);
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
        }
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
                inventory[i].item.SetActive(true);

                if (value < 1) return;
            }
        }

        Debug.LogError("All inventory slots unlocked. Couldn't expand inventory by " + value + " slots.");
    }

    // TODO Test if this Method is working
    public bool can_expand_slots()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].is_avaliable) return true;
        }

        return false;
    }
}
