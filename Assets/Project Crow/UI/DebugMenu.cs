using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    public void expand_inventory_capacity()
    {
        GI.player_inventory.expand_inventory_capacity();
    }

    // 0 for health
    // 1 for stamina
    // 2 for anti terror
    // 3 for anti overdose
    public void add_item(int index)
    {
        ItemDetails item_details = GI.item_data.get_item((ItemType)index);

        //GI.player_inventory.store_item((ItemType)actual_item);
    }
}
