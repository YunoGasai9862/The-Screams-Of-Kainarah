using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPouchManager : MonoBehaviour
{
    [SerializeField] InventoryPouchPanelEvent inventoryPouchPanelEvent;

    private void OnEnable()
    {
        inventoryPouchPanelEvent.Invoke(true);
    }

    private void OnDisable()
    {
        inventoryPouchPanelEvent.Invoke(false);
    }

}
