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

    [SerializeField] float rayLength;
    [SerializeField] RectTransform pickup_interface;
    [SerializeField] Image pickup_image;
    [SerializeField] RectTransform loot_interface;
    [SerializeField] TextMeshProUGUI pickup_text;

    void FixedUpdate()
    {
        if (GI.pause_game.game_paused) return;

        { // Display an item's details on screen
            bool has_hit_colliders = false;
            RaycastHit hit;

            has_hit_colliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, rayLength);
            
            // TODO Optimize this. Currently, this code is getting a component every frame that the raycast hits an collectable
            if (has_hit_colliders && hit.collider.gameObject.layer == 9) // Item layer
            {
                Item item = hit.collider.GetComponent<Item>();
                ItemDetails item_details = GI.item_data.get_details_of(item.type);

                // Sets the pickup ui position on the screen.
                Vector3 item_screen_position = GI.fp_camera.WorldToScreenPoint(hit.collider.transform.position);
                pickup_interface.position = item_screen_position;

                // Sets the pickup ui values.
                pickup_image.sprite = item_details.sprite;
                pickup_text.text = item_details.item_name;

                pickup_interface.gameObject.SetActive(true);
                item_found = hit.collider.gameObject;
            }
            else if (has_hit_colliders && hit.collider.CompareTag("Loot"))
            {
                item_found = hit.collider.gameObject;
                loot_interface.gameObject.SetActive(true);
            }
            else
            {
                loot_interface.gameObject.SetActive(false);
                pickup_interface.gameObject.SetActive(false);
                item_found = null;
            }
        }
    }

    void Update()
    {
        if (GI.pause_game.game_paused) return;

        { // Process Pickup Input
            if (Input.GetKeyDown(KeyCode.E) && (pickup_interface.gameObject.activeInHierarchy || loot_interface.gameObject.activeInHierarchy) )
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

                    //case "CollectableAmmo":
                    //    {
                    //        Ammo ammo = item_found.GetComponent<Ammo>();
                    //        bool has_added_ammo = GI.ammo_holster.store_or_remove(ammo.type, ammo.amount);
                    //        if (has_added_ammo) Destroy(item_found);
                    //        // ELSE give the player an feedbacck error
                    //    }
                    //    break;

                    //case "AmmoHolsterExpansion":
                    //    {
                    //        GI.ammo_holster.expand_max_ammo();
                    //        Destroy(item_found);
                    //    }
                    //    break;

                    case "InventoryExpansion":
                        {
                            if (!GI.player_inventory.can_expand_capacity()) break;

                            GI.player_inventory.expand_inventory_capacity();
                            Destroy(item_found);
                        }
                        break;

                    //case "CollectableGun":
                    //    {
                    //        Ammo gun = item_found.GetComponent<Ammo>();
                    //        GI.gun_switcher.collect_gun((int)gun.type);
                    //        Destroy(item_found);
                    //    }
                    //    break;
                        
                    //case "Loot":
                    //    {
                    //        item_found.SetActive(false);
                    //        int random_item = Random.Range(0, 2);

                    //        if (random_item == 0)
                    //        {
                    //            int random_consumable = Random.Range(0, (int)ConsumableType.COUNT);
                    //            int random_amount = Random.Range(1, 4);
                    //            // Maybe put every consumable into a list, and send an random item of the list
                    //            GI.player_inventory.store_or_remove((ConsumableType)random_consumable, random_amount);
                    //            Debug.Log("Collected " + random_amount + " of " + (ConsumableType)random_consumable);
                    //        }
                    //        else
                    //        {
                    //            int random_ammo_type = Random.Range(0, (int)AmmoType.COUNT);
                    //            int random_amount = Random.Range(1, 6);
                    //            GI.ammo_holster.store_or_remove((AmmoType)random_ammo_type, random_amount);
                    //            Debug.Log("Collected " + random_amount + " of " + (AmmoType)random_ammo_type);
                    //        }
                    //    }
                    //    break;

                    default:
                        Debug.LogError("Tag not found!");
                        break;
                }
            }
        }
    }
}
