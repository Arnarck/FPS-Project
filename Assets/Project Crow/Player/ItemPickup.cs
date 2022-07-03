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
    [SerializeField] TextMeshProUGUI pickup_text;

    void FixedUpdate()
    {
        { // Process Raycast
            bool has_hit_colliders = false;
            RaycastHit hit;

            has_hit_colliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, rayLength);

            if (has_hit_colliders)
            {
                if (hit.collider.CompareTag("Consumable") || hit.collider.CompareTag("CollectableAmmo") || hit.collider.CompareTag("InventoryExpansion") || hit.collider.CompareTag("AmmoHolsterExpansion"))
                {
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
                //else if (hit.collider.CompareTag("InventoryExpansion") || hit.collider.CompareTag("AmmoHolsterExpansion"))
                //{
                //    GameObject item = hit.collider.gameObject;

                //    // Sets the pickup ui position on the screen.
                //    Vector3 item_screen_position = GI.fp_camera.WorldToScreenPoint(item.transform.position);
                //    pickup_interface.position = item_screen_position;

                //    // Sets the pickup ui values.
                //    pickup_interface.gameObject.SetActive(true);
                //    item_found = hit.collider.gameObject;
                //}
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
                            bool has_added_ammo = GI.ammo_holster.increase_or_reduce(ammo.type, ammo.amount);
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

                    default:
                        Debug.LogError("Consumable not found!");
                        break;
                }
            }
        }
    }
}
