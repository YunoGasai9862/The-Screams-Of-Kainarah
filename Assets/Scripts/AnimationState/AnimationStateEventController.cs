using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AnimationStateEventController : StateMachineBehaviour
{
    [SerializeField] float invokeTime;
    [SerializeField] string animationEventName;
    [SerializeField] bool isUnityEventWithType;
    private EventsHelper _eventHelper = new EventsHelper();

    private bool _eventInvoke {get; set;}
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
                var customEvent = _eventHelper.GetCustomUnityEvent(SceneSingleton.EventStringMapper, animationEventName);
                customEvent.GetInstance().Invoke();
            }
            else
            {
                var customEvent = _eventHelper.GetCustomUnityEventWithType(SceneSingleton.EventStringMapper, animationEventName);
                customEvent.GetInstance().Invoke(true);
            }

        }
    }
}
