using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IObserver<GameState>, ISubject<IObserver<bool>>
{
    private const string DIAMOND_TAG = "Crystal";

    private const string FUNDS_TEXT_TAG = "DText";

    private GameObject m_insideObject;

    private AudioSource m_transact;

    private bool m_isSufficientFunds;
    private GameState CurrentGameState { get; set; }

    private TMPro.TextMeshProUGUI m_funds;

    private GlobalGameStateDelegator m_globalGameStateDelegator;

    private FlagDelegator m_genericFlagDelegator;

    private void Start()
    {
        m_funds = GameObject.FindGameObjectWithTag(FUNDS_TEXT_TAG).GetComponent<TMPro.TextMeshProUGUI>();

        m_genericFlagDelegator = Helper.GetDelegator<FlagDelegator>();

        m_globalGameStateDelegator = Helper.GetDelegator<GlobalGameStateDelegator>();

        m_globalGameStateDelegator.NotifySubjectWrapper(this, new NotificationContext()
        {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(GlobalGameStateManager).ToString()

        }, CancellationToken.None);

        m_genericFlagDelegator.AddToSubjectsDict(typeof(TriggerHandler).ToString(), gameObject.name, new Subject<IObserver<bool>>());

        m_genericFlagDelegator.GetSubsetSubjectsDictionary(typeof(TriggerHandler).ToString())[gameObject.name].SetSubject(this);

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
                m_isSufficientFunds = CheckIfFundsExists(m_funds);

                Debug.Log(m_isSufficientFunds);

                m_genericFlagDelegator.NotifyObservers(m_isSufficientFunds, gameObject.name, typeof(TriggerHandler), CancellationToken.None);

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

    public void OnNotify(GameState data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        CurrentGameState = data;
    }

    public void OnNotifySubject(IObserver<bool> data, NotificationContext notificationContext, CancellationToken cancellationToken, SemaphoreSlim semaphoreSlim, params object[] optional)
    {
        m_genericFlagDelegator.AddToSubjectObserversDict(gameObject.name, m_genericFlagDelegator.GetSubsetSubjectsDictionary(typeof(TriggerHandler).ToString())[gameObject.name],
           data);
    }
}
