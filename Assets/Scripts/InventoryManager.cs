using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using Assets.Scripts.Scene;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(InventoryManager), ContextType = typeof(bool))]
public class InventoryManager : Scene, IRequest<bool>
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
       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));
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
