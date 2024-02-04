using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFeedbackOnItem : MonoBehaviour, IPointerUpHandler, ISerializableFeildsHelper
{
    [SerializeField] string slotTag;

    public string FieldName { get => slotTag; set => slotTag = value; }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject item = eventData.pointerEnter.transform.gameObject;

        if (item.tag != slotTag)
        {
           InventoryManagementSystem.Instance.RemoveInventoryItemEvent.Invoke(item.tag);
        }
    }
}
