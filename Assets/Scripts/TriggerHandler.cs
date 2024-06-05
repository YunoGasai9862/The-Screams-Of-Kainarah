using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TriggerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const string DIAMOND_TAG = "Crystal";
    private GameObject m_insideObject;
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
            m_insideObject = eventData.pointerClick.transform.gameObject;
            if (m_insideObject.transform.childCount > 0)
            {
                if (CheckIfFundsExists(Funds))
                {
                    m_insideObject = m_insideObject.transform.GetChild(0).gameObject;
                    InventoryManagementSystem.Instance.AddInvoke(m_insideObject.GetComponent<SpriteRenderer>().sprite, m_insideObject.tag); //the rest of the process is automated in that function
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
    public void DecreaseFunds(ref TMPro.TextMeshProUGUI diamondText)
    {
        IncreaseCrystal.DiamondCount--;
        diamondText.text = IncreaseCrystal.DiamondCount.ToString("0");
        DecreaseDiamondsFromInventory();
    }

    public async void DecreaseDiamondsFromInventory()
    {
         string funds = await InventoryManagementSystem.Instance.GetItemTagFromInventoryToDecreaseFunds(DIAMOND_TAG);  
         InventoryManagementSystem.Instance.RemoveInvoke(funds);
    }
}
