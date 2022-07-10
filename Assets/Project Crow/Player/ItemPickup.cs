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
        { // Process Raycast
            bool has_hit_colliders = false;
            RaycastHit hit;

            has_hit_colliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, rayLength);

            if (has_hit_colliders && hit.collider.gameObject.layer == 9)
            {
                // Display item's details on screen
                CollectableItem item = hit.collider.GetComponent<CollectableItem>();

                // Sets the pickup ui position on the screen.
                Vector3 item_screen_position = GI.fp_camera.WorldToScreenPoint(item.transform.position);
                pickup_interface.position = item_screen_position;

                // Sets the pickup ui values.
                pickup_image = item.itemImage;
                pickup_text.text = item.itemName;

                pickup_interface.gameObject.SetActive(true);
                item_found = hit.collider.gameObject;
            }
            else if (has_hit_colliders && hit.collider.gameObject.layer == 10 && hit.collider.isTrigger) // TODO Improve this condition
            {
                item_found = hit.collider.gameObject;
                loot_interface.gameObject.SetActive(true);
            }
            else
            {
                pickup_interface.gameObject.SetActive(false);
                item_found = null;
            }
        }
    }

    void Update()
    {
        { // Process Pickup Input
            if (Input.GetKeyDown(KeyCode.E) && pickup_interface.gameObject.activeInHierarchy)
            {
                switch (item_found.tag)
                {
                    case "Consumable":
                        {
                            Consumable item = item_found.GetComponent<Consumable>();
                            bool has_stored_item = GI.player_inventory.store_or_remove(item.type, item.amount);
                            if (has_stored_item) Destroy(item_found);
                            // ELSE give the player an feedback error
                        }
                        break;

                    case "CollectableAmmo":
                        {
                            Ammo ammo = item_found.GetComponent<Ammo>();
                            bool has_added_ammo = GI.ammo_holster.store_or_remove(ammo.type, ammo.amount);
                            if (has_added_ammo) Destroy(item_found);
                            // ELSE give the player an feedbacck error
                        }
                        break;

                    case "AmmoHolsterExpansion":
                        {
                            GI.ammo_holster.expand_max_ammo();
                            Destroy(item_found);
                        }
                        break;

                    case "InventoryExpansion":
                        {
                            GI.player_inventory.expand_inventory_capacity();
                            Destroy(item_found);
                        }
                        break;

                    case "CollectableGun":
                        {
                            Ammo gun = item_found.GetComponent<Ammo>();
                            GI.gun_switcher.collect_gun((int)gun.type);
                            Destroy(item_found);
                        }
                        break;
                        
                    case "Enemy":
                        {
                            // loot
                            Destroy(item_found);
                            int random_item = Random.Range(0, 2);

                            if (random_item == 0)
                            {
                                int random_consumable = Random.Range(0, (int)ConsumableType.COUNT);
                                int random_amount = Random.Range(1, 4);
                                GI.player_inventory.store_or_remove((ConsumableType)random_consumable, random_amount);
                            }
                            else
                            {
                                int random_ammo_type = Random.Range(0, (int)AmmoType.COUNT);
                                int random_amount = Random.Range(1, 6);
                                GI.ammo_holster.store_or_remove((AmmoType)random_ammo_type, random_amount);
                            }
                        }
                        break;

                    default:
                        Debug.LogError("Consumable not found!");
                        break;
                }
            }
        }
    }
}
