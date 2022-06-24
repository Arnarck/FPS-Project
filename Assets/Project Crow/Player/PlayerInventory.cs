using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    int[] m_items_stored = new int[System.Enum.GetValues(typeof(ConsumableType)).Length];
    int[] m_max_items_storable = { 10, 10, 10, 10 };

    public void store_or_remove(ConsumableType consumable, int amount)
    {
        int item = (int)consumable;

        if (m_items_stored[item] >= m_max_items_storable[item]) return;

        m_items_stored[item] += amount;
        m_items_stored[item] = Mathf.Clamp(m_items_stored[item], 0, m_max_items_storable[item]);

        if (amount > 0) Debug.Log(amount + " " + consumable + " stored.");
        else Debug.Log(amount + " " + consumable + " removed.");
    }
}
