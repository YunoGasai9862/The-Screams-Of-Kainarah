using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using UnityEngine;

[Subject(SubjectType = typeof(InventoryManager), ContextType = typeof(InventoryManager))]
public class InventoryManager : MonoBehaviour, IRequest<InventoryManager>
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
    }

    public IEnumerator Request()
    {
        yield return StartCoroutine(Delegator.NotifyObservers(new SubjectContext<InventoryManager>() { Data = this, EntityType = typeof(InventoryManager) }, this));
    }
}
