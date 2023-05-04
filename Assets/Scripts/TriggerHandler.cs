using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject _insideObject;
    private Sprite _insideObjectSprite;
    public void OnPointerClick(PointerEventData eventData)
    {
        if(OpenWares.Buying)
        {
            _insideObject = eventData.pointerClick.transform.gameObject;
            _insideObjectSprite= _insideObject.GetComponent<Image>().sprite;
            CreateInventorySystem.AddToInventory(_insideObjectSprite, _insideObject.tag); //the rest of the process is automated in that function
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<Animator>().SetTrigger("isHighlight");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<Animator>().SetTrigger("isNotHighlight");

    }
}
