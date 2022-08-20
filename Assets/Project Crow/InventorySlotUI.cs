using UnityEngine;
using UnityEngine.UI;
using TMPro;

// @Arnarck change this name to "SlotData" or something like that
public class InventorySlotUI : MonoBehaviour
{
    public int index;
    public Button button;
    public TextMeshProUGUI count_text;

    public void clicked_item_slot()
    {
        // Disable item menu when clicking on the same slot that activated it
        if (GI.player_inventory.ui_item_menu.gameObject.activeInHierarchy && GI.player_inventory.current_slot_selected_on_item_menu == index)
        {
            GI.player_inventory.disable_item_menu();
            return;
        }
        
        if (!GI.player_inventory.combine_option_enabled) GI.player_inventory.toggle_item_menu(index);
        else GI.player_inventory.try_to_combine_item_slots(index);
    }
}
