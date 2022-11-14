using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DebugMenu : MonoBehaviour
{
    public bool insta_kill;
    public Button debug_button_prefab;
    public Transform avaliable_items_screen;
    public GameObject debug_screen;

    void Start()
    {
        // TODO Only do this stuff here if is Debug Build
        for (int i = 1; i < Enum.GetNames(typeof(ItemType)).Length - 1; i++) // Excludes NONE and COUNT
        {
            ItemType item_type = (ItemType)i;
            Button button = Instantiate(debug_button_prefab);
            button.transform.SetParent(avaliable_items_screen, false);
            button.onClick.AddListener(() => add_item((int)item_type)); // Converting from Enum to Int because i had problems using "i" variable as a parameter... It was always passing "0" as parameter, no matter "i" value.
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item_type.ToString();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            GI.pause_game.toggle_pause_game();
            debug_screen.SetActive(!debug_screen.activeSelf);
        }
    }

    public void toggle_debug_screen()
    {
        GI.pause_game.toggle_pause_game();
        debug_screen.SetActive(!debug_screen.activeSelf);
    }

    public void expand_inventory_capacity()
    {
        GI.player_inventory.expand_inventory_capacity();
    }

    public void toggle_avaliable_items()
    {
        avaliable_items_screen.gameObject.SetActive(!avaliable_items_screen.gameObject.activeSelf);
    }

    public void add_item(int index)
    {
        ItemType item_type = (ItemType)index;

        if (GI.player_inventory.is_cumulative(item_type)) GI.player_inventory.store_cumulative_item(item_type, InventoryData.max_capacity[index]);
        else GI.player_inventory.store_item(item_type);
    }

    public void kill_all_enemies()
    {
        foreach (EnemyAI enemy in GI.enemy_manager.enemies) enemy.take_damage(enemy.health);
    }

    public void toggle_insta_kill()
    {
        insta_kill = !insta_kill;
    }
}
