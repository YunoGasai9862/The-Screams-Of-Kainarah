using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryBagClickEvent : MonoBehaviour
{
    [SerializeField] RectTransform parentPosition;
    [SerializeField] Camera uiCamera;
    [SerializeField] RectTransform sourcePosition;
    [SerializeField] GraphicRaycaster graphicRayCaster;
    [SerializeField] EventSystem eventSystem;
    private PointerEventData m_pointerEventData;
    private List<RaycastResult> m_rayCastResult = new List<RaycastResult>();

    private Vector2 m_positionInCanvas;
    // Start is called before the first frame update
    void Start()
    {
        m_pointerEventData = new PointerEventData(eventSystem);
    }

   
    async void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentPosition, Input.mousePosition, uiCamera, out m_positionInCanvas);

        m_pointerEventData.position = m_positionInCanvas; //canvas position

        graphicRayCaster.Raycast(m_pointerEventData, m_rayCastResult);

        Debug.Log(m_rayCastResult.Count);

       // await IterateOverRayCasts(m_rayCastResult);


    }

    private async Task IterateOverRayCasts(List<RaycastResult> raycastResult)
    {
        foreach(var raycast in raycastResult)
        {
            Debug.Log(raycast);
            await Task.Delay(0);
        }
    }
}
