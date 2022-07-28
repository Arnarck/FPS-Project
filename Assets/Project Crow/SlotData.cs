using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotData : MonoBehaviour
{
    public int slot_index;
    public Button button;
    public TextMeshProUGUI count_text;

    public void remove_item()
    {
        GI.player_inventory.remove_item(slot_index);
    }
}
