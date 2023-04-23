using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFeedbackOnItem : MonoBehaviour, IPointerUpHandler
{
   

    public void OnPointerUp(PointerEventData eventData)
    {
        GameObject item = eventData.pointerEnter.transform.gameObject;
        CreateInventorySystem.ReduceItem(ref item);
    }
}
