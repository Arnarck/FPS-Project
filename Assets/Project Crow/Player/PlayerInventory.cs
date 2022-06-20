using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    int[] m_items_stored = new int[System.Enum.GetValues(typeof(ConsumableType)).Length];
    int[] m_max_items_stored = { 10, 10, 10, 10 };

    public void Store(ConsumableType consumable, int amount)
    {
        int item = (int)consumable;

        if (m_items_stored[item] >= m_max_items_stored[item]) return;
        if (amount < 0) amount *= -1;

        m_items_stored[item] += amount;
        m_items_stored[item] = Mathf.Clamp(m_items_stored[item], 0, m_max_items_stored[item]);
    }

    public void Remove(ConsumableType consumable, int amount)
    {
        int item = (int)consumable;

        if (m_items_stored[item] < 1) return;
        if (amount < 0) amount *= -1;

        m_items_stored[item] -= amount;
        m_items_stored[item] = Mathf.Clamp(m_items_stored[item], 0, m_max_items_stored[item]);
    }
}
