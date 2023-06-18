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
    private AudioSource transact;

    public static bool Failure=true;
    private void Start()
    {
        Funds = GameObject.FindGameObjectWithTag("DText").GetComponent<TMPro.TextMeshProUGUI>();
    }

    private void Update()
    {
        if(transact== null && OpenWares.Buying)
        {
            transact = GameObject.FindWithTag("Transact").GetComponent<AudioSource>();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OpenWares.Buying)
        {
            _insideObject = eventData.pointerClick.transform.gameObject;
            if (_insideObject.transform.childCount > 0)
            {
                if (CheckIfFundsExists(Funds))
                {
                    _insideObject = _insideObject.transform.GetChild(0).gameObject;
                    _insideObjectSprite = _insideObject.GetComponent<Image>().sprite;
                    CreateInventorySystem.AddToInventory(_insideObjectSprite, _insideObject.tag); //the rest of the process is automated in that function
                    transact.Play();
                    DecreaseFunds(ref Funds);
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
            if (funds == 0)
            {

                 Failure = false;
                 return false;
            }


        return true;

    }
    public void DecreaseFunds(ref TMPro.TextMeshProUGUI _text)
    {
        int funds= Int32.Parse(_text.text);
        funds--;
        IncreaseDiamond.count--;
        _text.text=funds.ToString("0");
        DecreaseDiamondsFromInventory();
    }

    public void DecreaseDiamondsFromInventory()
    {
        GameObject _diamondObject = CreateInventorySystem.CheckForObject("Crystal");
        if (_diamondObject != null)
        {
            GameObject _diamondObjectParent = _diamondObject.transform.parent.gameObject;
            CreateInventorySystem.ReduceItem(ref _diamondObjectParent, true);

        }

    }
}
