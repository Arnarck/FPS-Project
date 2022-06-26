using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    int[] items_stored = { 0, 0, 0, 0 };
    int[] max_items_storable = { 10, 10, 10, 10 };

    void Awake()
    {
        GI.player_inventory = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Food, -1);
            if (has_removed_item) GI.hunger.IncreaseValue(GC.HUNGER_RESTORED_BY_FOOD_ITEMS);
            // ELSE give the player an error feedback
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Water, -1);
            if (has_removed_item) GI.hunger.IncreaseValue(GC.THIRST_RESTORED_BY_WATER_ITEMS);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            bool has_removed_item = store_or_remove(ConsumableType.Medkit, -1);
            if (has_removed_item) GI.hunger.IncreaseValue(GC.HEALTH_RESTORED_BY_MEDKIT_ITEMS);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            // Consume battery
        }
    }

    public bool store_or_remove(ConsumableType consumable, int amount)
    {
        int item = (int)consumable;

        if (items_stored[item] >= max_items_storable[item]) return false; // Clamps max amount
        if (items_stored[item] < 1 && amount < 0) return false; // Clamps min amount

        items_stored[item] += amount;
        items_stored[item] = Mathf.Clamp(items_stored[item], 0, max_items_storable[item]);

        return true;
    }
}
