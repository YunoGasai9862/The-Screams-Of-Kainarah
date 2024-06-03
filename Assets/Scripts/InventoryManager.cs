using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;
    [SerializeField] InventoryPouchPanelEvent inventoryPouchPanelEvent;

    private void Start()
    {
        inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
        inventoryPouchPanelEvent.AddListener(IsPouchPanelActive);
    }
    public bool IsPouchOpen { get; set; } = false;

    public void ShouldInventoryBeVisible(bool visible)
    {
        InventoryPanel.SetActive(visible);
    }

    public void IsPouchPanelActive(bool isActive)
    {
        IsPouchOpen = isActive;
    }

}
