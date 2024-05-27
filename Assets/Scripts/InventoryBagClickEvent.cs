using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBagClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    private Vector2 m_positionInCanvas;

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICK");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTERING");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXITING");

    }


}
