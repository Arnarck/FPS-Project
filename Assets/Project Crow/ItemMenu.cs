using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemMenu : MonoBehaviour
{
    public Vector2 offset;
    public RectTransform m_rect_transform;
    public GridLayoutGroup m_grid_layout_group;
    [HideInInspector] public GameObject[] options;

    void Awake()
    {
        options = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            options[i] = transform.GetChild(i).gameObject;
        }

        gameObject.SetActive(false);
    }

    public void activate_item_menu()
    {
        int options_active = 0;

        foreach (GameObject option in options) if (option.activeSelf) options_active++;

        // Sets the height of the screen based on how many options will show
        m_rect_transform.sizeDelta = new Vector2(m_grid_layout_group.cellSize.x + offset.x, m_grid_layout_group.cellSize.y * options_active + offset.y);

        gameObject.SetActive(true);
    }
}
