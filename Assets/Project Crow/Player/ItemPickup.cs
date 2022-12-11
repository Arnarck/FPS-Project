using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum ConsumableType
{
    Food,
    Water,
    Medkit,
    FlashlightBattery,

    COUNT
}

public class ItemPickup : MonoBehaviour
{
    GameObject item_found;

    public float ray_length = 2f;

    void FixedUpdate()
    {
        if (GI.pause_game.game_paused) return;

        { // Display an item's details on screen
            bool has_hit_colliders = false;
            RaycastHit hit;

            has_hit_colliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, ray_length);
            
            // @TODO: Optimize this. Currently, this code is getting a component every frame that the raycast hits an collectable
            if (has_hit_colliders && hit.collider.gameObject.layer == 9) // Item layer
            {
                Item item = hit.collider.GetComponent<Item>();
                ItemDetails item_details = GI.item_data.get_details_of(item.type);

                // Sets the pickup ui position on the screen.
                Vector3 item_screen_position = GI.fp_camera.WorldToScreenPoint(hit.collider.transform.position);
                GI.hud.pickup_interface.position = item_screen_position;

                // Sets the pickup ui values.
                GI.hud.pickup_image.sprite = item_details.sprite;
                GI.hud.pickup_text.text = item_details.item_name;

                GI.hud.pickup_interface.gameObject.SetActive(true);
                item_found = hit.collider.gameObject;
            }
            else
            {
                GI.hud.pickup_interface.gameObject.SetActive(false);
                item_found = null;
            }
        }
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Process Pickup Input
            if (Input.GetKeyDown(KeyCode.E) && GI.hud.pickup_interface.gameObject.activeInHierarchy)
            {
                switch (item_found.tag)
                {
                    case "Item":
                        {
                            Item item = item_found.GetComponent<Item>();
                            if (GI.player_inventory.is_cumulative(item.type))
                            {
                                Debug.Log($"{item.type} is a cumulative item!");
                                bool has_stored_item_completely = GI.player_inventory.store_cumulative_item(item.type, item.amount);
                                if (has_stored_item_completely) Destroy(item.gameObject);
                            }
                            else
                            {
                                Debug.Log($"{item.type} is a single item!");
                                bool has_stored_item = GI.player_inventory.store_item(item.type);
                                if (has_stored_item) Destroy(item.gameObject);
                            }

                            if (GI.player_inventory.check_if_item_is_ammo_and_corresponds_to_equiped_weapon(item.type))
                                GI.player_inventory.display_total_ammo_of_equiped_weapon();
                        }
                        break;

                    case "InventoryExpansion":
                        {
                            if (!GI.player_inventory.can_expand_capacity()) break;

                            GI.player_inventory.expand_inventory_capacity();
                            Destroy(item_found);
                        }
                        break;

                    default:
                        Debug.LogError("Tag not found!");
                        break;
                }
            }
        }
    }
}
