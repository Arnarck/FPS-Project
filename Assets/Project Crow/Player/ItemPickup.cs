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
    [SerializeField] GameObject pickup_display;
    [SerializeField] Image pickup_image;
    [SerializeField] TextMeshProUGUI pickup_text;

    void FixedUpdate()
    {
        { // Process Raycast
            bool has_hit_colliders = false;
            RaycastHit hit;

            has_hit_colliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, rayLength);

            if (has_hit_colliders && (hit.collider.CompareTag("Consumable") || hit.collider.CompareTag("CollectableAmmo")))
            {
                CollectableItem item = hit.collider.GetComponent<CollectableItem>();

                // Sets the pickup ui position on the screen.
                Vector3 item_screen_position = GI.fp_camera.WorldToScreenPoint(item.transform.position);
                pickup_display.GetComponent<RectTransform>().position = item_screen_position;

                // Sets the pickup ui values.
                pickup_image = item.itemImage;
                pickup_text.text = item.itemName;

                pickup_display.SetActive(true);
                item_found = hit.collider.gameObject;
            }
            else
            {
                pickup_display.SetActive(false);
                item_found = null;
            }
        }
    }

    void Update()
    {
        {// Process Pickup Input
            if (Input.GetKeyDown(KeyCode.E) && pickup_display.activeInHierarchy)
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
                            GI.ammo_holster.IncreaseAmmo(ammo.type, ammo.amount);
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
