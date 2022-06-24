using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConsumableType
{
    Food,
    Water,
    Medkit,
    FlashlightBattery
}

public class ItemPickup : MonoBehaviour
{
    GameObject m_item_found;

    [SerializeField] float rayLength;
    [SerializeField] GameObject collectUI;

    void FixedUpdate()
    {
        { // Process Raycast
            bool hasHitColliders = false;
            RaycastHit hit;

            hasHitColliders = Physics.Raycast(GI.fp_camera.transform.position, GI.fp_camera.transform.forward, out hit, rayLength);

            if (hasHitColliders && (hit.collider.CompareTag("Consumable") || hit.collider.CompareTag("CollectableAmmo")))
            {
                collectUI.SetActive(true);
                m_item_found = hit.collider.gameObject;
            }
            else
            {
                collectUI.SetActive(false);
                m_item_found = null;
            }
        }
    }

    void Update()
    {
        {// Process Pickup Input
            if (Input.GetKeyDown(KeyCode.E) && collectUI.activeInHierarchy)
            {
                switch (m_item_found.tag)
                {
                    case "Consumable":
                        Consumable item = m_item_found.GetComponent<Consumable>();
                        break;

                    case "CollectableAmmo":
                        {
                            Ammo ammo = m_item_found.GetComponent<Ammo>();
                            GI.ammo_holster.IncreaseAmmo(ammo.type, ammo.amount);
                        }
                        break;
                }
                Destroy(m_item_found);
            }
        }
    }
}
