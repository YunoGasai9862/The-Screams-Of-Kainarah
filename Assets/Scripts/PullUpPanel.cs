using System.Collections;
using System.Threading;
using UnityEngine;

public class PullUpPanel : MonoBehaviour, IObserver<bool>
{
    private const string SUFFICIENT_FUNDS_ANIMATION_CONDITION = "SufficientFunds";

    private const float WAITING_TIME = 1.0f;

    private Animator m_anim;

    [SerializeField]
    private FlagDelegator flagDelegator;

    void Start()
    {
        m_anim = GetComponent<Animator>();

        StartCoroutine(flagDelegator.NotifySubject(this, new NotificationContext() {
            ObserverName = this.name,
            ObserverTag = this.name,
            SubjectType = typeof(TriggerHandler).ToString()

        }, CancellationToken.None));
    }

    IEnumerator RunAnimation(bool data, float waitingTime)
    {
        m_anim.SetBool(SUFFICIENT_FUNDS_ANIMATION_CONDITION, data);

        yield return new WaitForSeconds(waitingTime);

        m_anim.SetBool(SUFFICIENT_FUNDS_ANIMATION_CONDITION, !data);
    }

    public void OnNotify(bool data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        if (data)
        {
            return;
        }

        StartCoroutine(RunAnimation(data, WAITING_TIME));
    }
}
