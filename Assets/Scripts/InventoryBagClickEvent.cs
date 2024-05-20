using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryBagClickEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Input.mousePosition);
    }
}
