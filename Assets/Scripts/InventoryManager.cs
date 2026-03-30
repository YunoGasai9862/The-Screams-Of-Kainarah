using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(InventoryManager), ContextType = typeof(bool))]
public class InventoryManager : MonoBehaviour, IRequest<bool>
{
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;
    [SerializeField] InventoryPouchPanelEvent inventoryPouchPanelEvent;

    private Delegator Delegator { get; set; }

    private void Start()
    {
        inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
        inventoryPouchPanelEvent.AddListener(IsPouchPanelActive);
    }

    private async void Awake()
    {
        Delegator = await Helper.GetDelegator<Delegator>();
    }
    public bool IsPouchOpen { get; set; } = false;

    public void ShouldInventoryBeVisible(bool visible)
    {
        InventoryPanel.SetActive(visible);
    }

    public void IsPouchPanelActive(bool isActive)
    {
        IsPouchOpen = isActive;

       Delegator.NotifyObserversWrapper(new SubjectContext<bool>() { Data = isActive, EntityType = typeof(InventoryManager) }, this);
    }
}
