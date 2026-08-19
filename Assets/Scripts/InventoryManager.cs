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

    private SceneUtils SceneUtils { get; set; }

    private async void Start()
    {
        await inventoryPouchClickEvent.AddListener(ShouldInventoryBeVisible);
        await inventoryPouchPanelEvent.AddListener(IsPouchPanelActive);

        SceneUtils = await (await GetBaseScene()).GetSceneUtilsAsync();
    }

    public bool IsPouchOpen { get; set; } = false;

    public void ShouldInventoryBeVisible(bool visible)
    {
        InventoryPanel.SetActive(visible);
    }

    public void IsPouchPanelActive(bool isActive)
    {
        IsPouchOpen = isActive;

        StartCoroutine(SceneUtils.NotifyObserversWrapper(new SubjectContext<bool>() { Data = isActive, EntityType = typeof(InventoryManager) }, this));
    }
}
