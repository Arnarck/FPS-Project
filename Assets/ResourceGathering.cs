using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Medkit,
    Food,
    Water,
    Ammo
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

        if (hasHitColliders && hit.collider.CompareTag("Collectable"))
        {
            _itemFound = hit.collider.gameObject;
            collectUI.SetActive(true);
        }
        else
        {
            _itemFound = null;
            collectUI.SetActive(false);
        }
    }

    void ProcessGatherInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && collectUI.activeInHierarchy)
        {
            Destroy(_itemFound);
        }
    }
}
