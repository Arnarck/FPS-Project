using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    GameObject[] avaliable_weapons;

    // Items (Collectables)
    int[] items_stored = { 0, 0, 0, 0 };
    int[] max_items_storable = { 5, 5, 3, 5 };
    int[] expanded_amount = { 5, 5, 3, 5 };

    [SerializeField] TextMeshProUGUI[] item_amount_text;
    public bool is_knife_equiped;

    void Awake()
    {
        GI.player_inventory = this;
    }

    void Start()
    {
        for (int i = 0; i < items_stored.Length; i++)
        {
            item_amount_text[i].text = items_stored[i].ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Food, -1);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Water, -1);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Medkit, -1);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            bool has_removed_item = store_or_remove(ConsumableType.FlashlightBattery, -1);
            if (has_removed_item) GI.player_flashlight.add_battery(100);
        }
    }

    public bool store_or_remove(ConsumableType consumable, int amount)
    {
        int item = (int)consumable;

        if (amount > 0 && items_stored[item] >= max_items_storable[item]) return false; // Clamps max amount
        if (amount < 0 && items_stored[item] < 1) return false; // Clamps min amount

        items_stored[item] += amount;
        items_stored[item] = Mathf.Clamp(items_stored[item], 0, max_items_storable[item]);
        item_amount_text[item].text = items_stored[item].ToString();

        return true;
    }

    public void expand_inventory_capacity()
    {
        for(int i = 0; i < max_items_storable.Length; i++)
        {
            max_items_storable[i] += expanded_amount[i];
            Debug.Log((ConsumableType)i + " expanded to " + max_items_storable[i]);
        }
    }
}
