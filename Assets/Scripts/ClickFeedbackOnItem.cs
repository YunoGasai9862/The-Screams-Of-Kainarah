using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFeedbackOnItem : MonoBehaviour, IPointerUpHandler, ISerializableFeildsHelper
{
    [SerializeField] string SlotTag;

    public string FieldName { get => SlotTag; set => SlotTag = value; }

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject item = eventData.pointerEnter.transform.gameObject;

        if (item.tag != SlotTag)
        {
            CreateInventorySystem.ReduceItem(ref item, false);

        }
    }
}
