using Assets.Annotations;
using System.Collections;
using UnityEngine;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using Annotations.Enums;
using Assets.Scripts.BaseScene;

[Observer(AssetType = Asset.MONOBEHAVIOR, EntityType = typeof(PullUpPanel), SubjectType = typeof(TriggerHandler), ContextType = typeof(bool))]
public class PullUpPanel : MonoBehaviorScene, INotify<bool>
{
    private const string SUFFICIENT_FUNDS_ANIMATION_CONDITION = "SufficientFunds";

    private const float WAITING_TIME = 1.0f;

    private Animator m_anim;

    private Delegator Delegator { get; set; }

    private SceneUtils SceneUtils { get; set; }

    private async void Awake()
    {
        SceneUtils = await GetBaseScene().GetSceneUtilsAsync();

        StartCoroutine(SceneUtils.GetDelegator<Delegator>(value => Delegator = value));
    }

    void Start()
    {
        m_anim = GetComponent<Animator>();

        StartCoroutine(Delegator.NotifySubject(new ObserverContext<bool>() {
            EntityType = typeof(PullUpPanel),
            Instance = gameObject,
            SubjectType = typeof(TriggerHandler)

        }, this));
    }

    IEnumerator RunAnimation(bool data, float waitingTime)
    {
        m_anim.SetBool(SUFFICIENT_FUNDS_ANIMATION_CONDITION, data);

        yield return new WaitForSeconds(waitingTime);

        m_anim.SetBool(SUFFICIENT_FUNDS_ANIMATION_CONDITION, !data);
    }

    public IEnumerator Notify(bool value)
    {
        if (value)
        {
            yield return null;
        }

        yield return StartCoroutine(RunAnimation(value, WAITING_TIME));
    }
}
