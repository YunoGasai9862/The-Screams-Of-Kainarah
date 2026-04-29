using Assets.Annotations;
using Assets.Scripts.Interfaces.Mediator.EnhancedV1;
using System.Collections;
using System.Linq;
using UnityEngine;
using Annotations.Enums;

[Observer(AssetType = Asset.PLAYER_STATE_MACHINE, EntityType = typeof(AnimationStateEventController), SubjectType = typeof(EntityPoolManager), ContextType = typeof(EntityPoolManager))]
public class AnimationStateEventController : StateMachineBehaviour, INotify<EntityPoolManager>
{
    private const string EVENT_STRING_MAPPER_CONFIG_KEY = "EventStringMapper";

    [SerializeField] float invokeTime;
    [SerializeField] string animationEventName;
    [SerializeField] bool isUnityEventWithType;
    private EventsHelper _eventHelper = new EventsHelper();

    private bool _eventInvoke {get; set;}

    private EntityPoolManager EntityPoolManagerInstance {get; set;}

    private EventStringMapper EventStringMapperConfig { get; set; }

    public IEnumerator Notify(EntityPoolManager value)
    {
        EntityPoolManagerInstance = value;

        EventStringMapperConfig = (EventStringMapper) EntityPoolManagerInstance.GetPooledEntity(EVENT_STRING_MAPPER_CONFIG_KEY).FirstOrDefault(entity => entity.Name.Equals(EVENT_STRING_MAPPER_CONFIG_KEY)).Entity;

        yield return null;

    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _eventInvoke = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        float animationTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        if (animationTime > invokeTime && !_eventInvoke)
        {
            _eventInvoke = true;
            if (!isUnityEventWithType)
            {
                var customEvent = _eventHelper.GetCustomUnityEvent(EventStringMapperConfig, animationEventName);
                customEvent.GetInstance().Invoke();
            }
            else
            {
                var customEvent = _eventHelper.GetCustomUnityEventWithType(EventStringMapperConfig, animationEventName);
                customEvent.GetInstance().Invoke(true);
            }

        }
    }
}
