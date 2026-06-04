using Assets.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using UnityEngine.EventSystems;
using Annotations.Enums;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(TriggerHandler), SubjectType = typeof(GameStateConsumer), ContextType = typeof(GenericStateBundle<GameStateBundle>))]
[Subject(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(TriggerHandler), ContextType = typeof(bool))]
public class TriggerHandler : Scene, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, INotify<GenericStateBundle<GameStateBundle>>, Assets.Scripts.Interfaces.Mediator.EnhancedV2.IRequest<bool>
{
    private const string DIAMOND_TAG = "Crystal";

    private const string FUNDS_TEXT_TAG = "DText";

    private GameObject m_insideObject;

    private AudioSource m_transact;

    private bool m_isSufficientFunds;
    private GenericStateBundle<GameStateBundle> CurrentGameState { get; set; } = new GenericStateBundle<GameStateBundle>();

    private TMPro.TextMeshProUGUI m_funds;

    private Delegator Delegator { get; set; }

    private async void Start()
    {
        m_funds = GameObject.FindGameObjectWithTag(FUNDS_TEXT_TAG).GetComponent<TMPro.TextMeshProUGUI>();

       StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));

        Delegator.NotifySubjectWrapper(new ObserverContext<GenericStateBundle<GameStateBundle>>()
        {
            Instance = gameObject,
            EntityType = typeof(TriggerHandler),
            SubjectType = typeof(GameStateConsumer)

        }, this);
    }

    private void Update()
    {
        if(m_transact == null && CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.SHOPPING))
        {
            m_transact = GameObject.FindWithTag("Transact").GetComponent<AudioSource>();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentGameState.StateBundle.GameState.CurrentState.Equals(GameState.SHOPPING))
        {
            m_insideObject = eventData.pointerClick.transform.gameObject;

            if (m_insideObject.transform.childCount > 0)
            {
                m_isSufficientFunds = CheckIfFundsExists(m_funds);

                Debug.Log(m_isSufficientFunds);

                Delegator.NotifyObserversWrapper(new SubjectContext<bool>() { Data = m_isSufficientFunds, EntityType = typeof(TriggerHandler) }, this);

                if (m_isSufficientFunds)
                {
                    m_insideObject = m_insideObject.transform.GetChild(0).gameObject;

                    InventoryManagementSystem.Instance.AddInvoke(m_insideObject.GetComponent<SpriteRenderer>().sprite, m_insideObject.tag); //the rest of the process is automated in that function

                    m_transact.Play();

                    DecreaseFunds(ref m_funds);
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

    public bool CheckIfFundsExists(TMPro.TextMeshProUGUI fundsText)
    {
        int funds = Int32.Parse(fundsText.text);

        if (funds == 0)
        {
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

    public IEnumerator Notify(GenericStateBundle<GameStateBundle> value)
    {
        CurrentGameState.StateBundle = value.StateBundle;

        yield return null;
    }

    public IEnumerator<bool> Request()
    {
        StartCoroutine(Delegator.NotifyObservers(new SubjectContext<bool>() { Data = m_isSufficientFunds, EntityType = typeof(TriggerHandler) }, this));

        yield return true;
    }
}
