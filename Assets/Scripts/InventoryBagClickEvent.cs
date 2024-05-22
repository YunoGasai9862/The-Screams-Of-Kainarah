using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBagClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] RectTransform parentPosition;
    [SerializeField] Camera uiCamera;
    [SerializeField] RectTransform sourcePosition;
    [SerializeField] GraphicRaycaster graphicRayCaster;
    [SerializeField] EventSystem eventSystem;
    private PointerEventData m_pointerEventData;

    private Vector2 m_positionInCanvas;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("CLICKING");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("ENTERING");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("EXITING");
    }

    // Start is called before the first frame update
    void Start()
    {
        m_pointerEventData = new PointerEventData(eventSystem);
    }

   
    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentPosition, Input.mousePosition, uiCamera, out m_positionInCanvas);

        m_pointerEventData.position = m_positionInCanvas; //canvas position

        Debug.Log(m_positionInCanvas);

    }
}
