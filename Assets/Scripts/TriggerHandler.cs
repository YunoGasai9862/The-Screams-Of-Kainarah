using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IObserver<GameState>, ISubject<IObserver<bool>>
{
    private const string DIAMOND_TAG = "Crystal";
    private GameObject m_insideObject;
    private AudioSource m_transact;
    private bool m_isSufficientFunds;
    private GameState CurrentGameState { get; set; }

    [SerializeField]
    TMPro.TextMeshProUGUI funds;
    [SerializeField] 
    GlobalGameStateDelegator globalGameStateDelegator;
    [SerializeField]
    GenericFlagDelegator genericFlagDelegator;

    private void Start()
    {
        funds = GameObject.FindGameObjectWithTag("DText").GetComponent<TMPro.TextMeshProUGUI>();

        globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);

        genericFlagDelegator.AddToSubjectsDict(typeof(TriggerHandler).ToString(), gameObject.name, new Subject<IObserver<bool>>());

        genericFlagDelegator.GetSubsetSubjectsDictionary(typeof(TriggerHandler).ToString())[gameObject.name].SetSubject(this);

    }

    private void Update()
    {
        if(m_transact == null && CurrentGameState.Equals(GameState.SHOPPING))
        {
            m_transact = GameObject.FindWithTag("Transact").GetComponent<AudioSource>();
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (CurrentGameState.Equals(GameState.SHOPPING))
        {
            m_insideObject = eventData.pointerClick.transform.gameObject;

            if (m_insideObject.transform.childCount > 0)
            {
                m_isSufficientFunds = CheckIfFundsExists(funds);

                if (m_isSufficientFunds)
                {
                    m_insideObject = m_insideObject.transform.GetChild(0).gameObject;
                    InventoryManagementSystem.Instance.AddInvoke(m_insideObject.GetComponent<SpriteRenderer>().sprite, m_insideObject.tag); //the rest of the process is automated in that function
                    m_transact.Play();
                    DecreaseFunds(ref funds);
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

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public void OnNotifySubject(IObserver<bool> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        genericFlagDelegator.AddToSubjectObserversDict(gameObject.name, genericFlagDelegator.GetSubsetSubjectsDictionary(typeof(TriggerHandler).ToString())[gameObject.name],
           data);
    }

    public void PingListeners(bool sufficientFunds)
    {
        foreach(Association<bool> association in genericFlagDelegator.GetSubjectAssociations(gameObject.name))
        {
            genericFlagDelegator.NotifyObserver(association.Observer, sufficientFunds, new NotificationContext()
            {
                SubjectType = typeof(TriggerHandler).ToString()
            }, CancellationToken.None);
        }
    }
}
