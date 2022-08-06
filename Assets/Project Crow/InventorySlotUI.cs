using UnityEngine;
using UnityEngine.UI;
using TMPro;

// TODO change this name to "SlotData" or something like that
public class InventorySlotUI : MonoBehaviour
{
    public int index;
    public Button button;
    public TextMeshProUGUI count_text;

    public void remove_item()
    {
        GI.player_inventory.remove_item(index);
    }

    public void toggle_item_menu()
    {
        GI.player_inventory.toggle_item_menu(index);
    }
}
