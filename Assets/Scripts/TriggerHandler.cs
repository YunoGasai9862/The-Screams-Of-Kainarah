using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.AssemblyQualifiedNameParser;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private GameObject _insideObject;
    private Sprite _insideObjectSprite;
    [SerializeField] TMPro.TextMeshProUGUI Funds;

    public static bool Failure;
    private void Start()
    {
       
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if(OpenWares.Buying)
        {
            _insideObject = eventData.pointerClick.transform.gameObject;
            if (_insideObject.transform.childCount>0)
            {
                if(CheckIfFundsExists(Funds))
                {
                    _insideObject = _insideObject.transform.GetChild(0).gameObject;
                    _insideObjectSprite = _insideObject.GetComponent<Image>().sprite;
                    CreateInventorySystem.AddToInventory(_insideObjectSprite, _insideObject.tag); //the rest of the process is automated in that function
                }else
                {
                    //_anim.SetBool("")
                }
                

            }

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

    public bool CheckIfFundsExists(TMPro.TextMeshProUGUI _text)
    {
        int funds = Int32.Parse(_text.text);
        if(funds==0)
        {
            Failure = false;
            return false;
        }

        return true;
    }
}
