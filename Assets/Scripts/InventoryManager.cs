using Annotations.Enums;
using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.Base;
using Assets.Scripts.BaseScene;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(InventoryManager), ContextType = typeof(bool))]
public class InventoryManager : MonoBehaviorScene, IRequest<bool>
{
    [SerializeField] GameObject InventoryPanel;
    [SerializeField] InventoryPouchClickEvent inventoryPouchClickEvent;
    [SerializeField] InventoryPouchPanelEvent inventoryPouchPanelEvent;

    private Delegator Delegator { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Start()
    {
        await inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
        await inventoryPouchPanelEvent.AddListener(IsPouchPanelActive);

        SceneUtils = await BaseScene.GetSceneUtilsAsync();

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
