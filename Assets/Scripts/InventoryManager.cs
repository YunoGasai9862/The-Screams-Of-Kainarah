using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;

    private void Start()
    {
        inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
    }

    public bool IsPouchOpen { get; set; } = false;
    public void HandleInventory()
    {
        if (!IsPouchOpen && !OpenWares.Buying)
        {
            InventoryPanel.SetActive(true);
            IsPouchOpen = true;
        }
        else
        {
            InventoryPanel.SetActive(false);
            IsPouchOpen = false;
        }

    }

    public void ShouldInventoryBeVisible(bool visible)
    {
        InventoryPanel.SetActive(visible);
    }

}
