using PlayerAnimationHandler;
using System.Threading;
using UnityEngine;

//convert it to a controller
public class PlayerAnimationController : MonoBehaviour, IReceiver<ActionExecuted>, IObserver<GenericStateBundle<PlayerStateBundle>>
{
    private AnimationStateMachine _stateMachine;

    private Animator _anim;

    private float _maxSlideTime = 0.4f;

    private GenericStateBundle<PlayerStateBundle> PlayerStateBundle { get; set; } = new GenericStateBundle<PlayerStateBundle>();

    private PlayerStateDelegator PlayerStateDelegator { get; set; }

    private PlayerStateEvent PlayerStateEvent { get; set; }

    private void Awake()
    {
        _stateMachine = new AnimationStateMachine(GetComponent<Animator>());

        PlayerStateDelegator = Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateEvent = Helper.GetCustomEvent<PlayerStateEvent>();

        if (PlayerStateDelegator == null)
        {
            throw new DelegatorNotFoundException("PlayerStateDelegator not found!!");
        }

        if (PlayerStateEvent == null)
        {
            throw new CustomEventNotFoundException("PlayerStateEvent not found!!");
        }
    }

    private void Start()
    {
        StartCoroutine(PlayerStateDelegator.NotifySubject(this, new NotificationContext()
        {
            ObserverName = gameObject.name,
            ObserverTag = gameObject.tag,
            SubjectType = typeof(PlayerStateConsumer).ToString()
        }, CancellationToken.None));

    }

    //CONTROLELR SHOULD NOT BE DOING THIS - I NEED TO REMOVE THIS 
    private void Update()
    {
        //REMOVE THIS FROM HERE!!!!!!!!!!
        if (_anim != null && _anim.GetCurrentAnimatorStateInfo(0).IsName(PlayerAnimationConstants.SLIDING) &&
            ReturnCurrentAnimation() > _maxSlideTime)
        {
            PlayAnimation(PlayerAnimationConstants.SLIDING, false);  //for fixing the Sliding Issue
        }
    }

    private bool VectorChecker(float compositionX)
    {
        return compositionX != 0f;
    }

    private void PlayAnimation(string name, int state)
    {
        _stateMachine.AnimationPlayForInt(name, state);
    }
    private void PlayAnimation(string name, bool state)
    {
        _stateMachine.AnimationPlayForBool(name, state);
    }
    private void PlayAnimation(string name, float state)
    {
        _stateMachine.AnimationPlayForFloat(name, state);
    }

    private void MovementAnimation(float keystroke)
    {

        PlayerStateBundle.StateBundle.PlayerMovementState = new State<PlayerMovementState>() { CurrentState = (VectorChecker(keystroke) && !PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState.Equals(PlayerMovementState.IS_JUMPING)) ?
            PlayerMovementState.IS_RUNNING : PlayerMovementState.IS_WALKING, IsConcluded = false };

        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int)PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState);
    }

    private void JumpingFallingAnimationHandler(bool keystroke)
    {
        PlayerStateBundle.StateBundle.PlayerMovementState = keystroke ?
            new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_JUMPING, IsConcluded = false } : 
            new State<PlayerMovementState>() { CurrentState = PlayerMovementState.IS_FALLING, IsConcluded = false };

        PlayAnimation(PlayerAnimationConstants.MOVEMENT, (int)PlayerStateBundle.StateBundle.PlayerMovementState.CurrentState);
    }
    private void UpdateJumpTime(string parameterName, float jumpTime)
    {
        PlayAnimation(parameterName, jumpTime);
    }

    private void Sliding(bool keystroke)
    {
        PlayAnimation(PlayerAnimationConstants.SLIDING, keystroke);
    }

    private float ReturnCurrentAnimation()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    private bool IsNameOfTheCurrentAnimation(string name)
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    private Animator getAnimator()
    {
        return _anim;
    }

    public void OnNotify(GenericStateBundle<PlayerStateBundle> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        PlayerStateBundle.StateBundle = data.StateBundle;
    }

    public ActionExecuted PerformAction(ActionExecuted value = default)
    {
        throw new System.NotImplementedException();
    }

    public ActionExecuted CancelAction()
    {
        throw new System.NotImplementedException();
    }
}