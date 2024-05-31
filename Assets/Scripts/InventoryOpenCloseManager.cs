using UnityEngine;

public class InventoryOpenCloseManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;

    private void Start()
    {
        inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
    }

    public bool isOpenInventory = false;
    public void HandleInventory()
    {
        if (!isOpenInventory && !OpenWares.Buying)
        {
            InventoryPanel.SetActive(true);
            isOpenInventory = true;
        }
        else
        {
            InventoryPanel.SetActive(false);
            isOpenInventory = false;
        }

    }

    public void ShouldInventoryBeVisible(bool visible)
    {

    }

}
