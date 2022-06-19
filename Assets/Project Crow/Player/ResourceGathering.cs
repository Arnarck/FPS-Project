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

public class ResourceGathering : MonoBehaviour
{
    GameObject _itemFound;

    [SerializeField] float rayLength;
    [SerializeField] GameObject collectUI;

    void FixedUpdate()
    {
        ProcessRaycast();
    }

    void Update()
    {
        ProcessGatherInput();
    }

    void ProcessRaycast()
    {
        bool hasHitColliders = false;
        RaycastHit hit;

        hasHitColliders = Physics.Raycast(GI.Instance.FPCamera.transform.position, GI.Instance.FPCamera.transform.forward, out hit, rayLength);

        if (hasHitColliders && (hit.collider.CompareTag("Consumable") || hit.collider.CompareTag("CollectableAmmo")) )
        {
            collectUI.SetActive(true);
            _itemFound = hit.collider.gameObject;
        }
        else
        {
            collectUI.SetActive(false);
            _itemFound = null;
        }
    }

    void ProcessGatherInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && collectUI.activeInHierarchy)
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        switch (_itemFound.tag)
        {
            case "Consumable":
                // Add consumable to inventory.
                break;

            case "CollectableAmmo":
                {
                    Ammo collectableAmmo = _itemFound.GetComponent<Ammo>();
                    GI.Instance.ammoHolster.IncreaseAmmo(collectableAmmo.type, collectableAmmo.amount);
                }
                break;
        }
        Destroy(_itemFound);
    }
}
