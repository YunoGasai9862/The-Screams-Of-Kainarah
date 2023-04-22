using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickFeedbackOnItem : MonoBehaviour, IPointerUpHandler
{
   

    public void OnPointerUp(PointerEventData eventData)
    {
        CreateInventorySystem.ReduceItem(eventData.pointerEnter.transform.gameObject);
    }
}
