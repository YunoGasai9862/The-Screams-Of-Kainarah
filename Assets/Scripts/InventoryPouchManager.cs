using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPouchManager : Scene
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
