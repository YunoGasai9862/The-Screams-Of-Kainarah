using PlayerAnimationHandler;
using System.Threading;
using UnityEngine;

public class PlayerAnimationMethods : MonoBehaviour, IObserver<GenericState<PlayerState>>
{
    private AnimationStateMachine _stateMachine;

    private Animator _anim;

    private float _maxSlideTime = 0.4f;

    private GenericState<PlayerState> CurrentPlayerState { get; set; } = new GenericState<PlayerState>();

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

    private void Update()
    {
        if (_anim != null && _anim.GetCurrentAnimatorStateInfo(0).IsName(AnimationConstants.SLIDING) &&
            ReturnCurrentAnimation() > _maxSlideTime)
        {
            PlayAnimation(AnimationConstants.SLIDING, false);  //for fixing the Sliding Issue
        }

    }

    public bool VectorChecker(float compositionX)
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
    public void RunningWalkingAnimation(float keystroke)
    {
        if (VectorChecker(keystroke) && !CurrentPlayerState.State.Equals(PlayerState.IS_JUMPING))
        {
            UpdateMovementState(AnimationStateKeeper.StateKeeper.RUNNING, true, false);

        }

        if (!VectorChecker(keystroke) && !CurrentPlayerState.State.Equals(PlayerState.IS_JUMPING))
        {
            UpdateMovementState(AnimationStateKeeper.StateKeeper.IDLE, false, true);
        }

    }

    public void UpdateMovementState(AnimationStateKeeper.StateKeeper state, bool isRunning, bool isWalking)
    {
        AnimationStateKeeper.CurrentPlayerState = (int)state;
        SetMovementStates(isRunning, isWalking);
        PlayAnimation(AnimationConstants.MOVEMENT, AnimationStateKeeper.CurrentPlayerState);
    }

    public void JumpingFallingAnimationHandler(bool keystroke)
    {
        AnimationStateKeeper.CurrentPlayerState = keystroke
            ? (int)AnimationStateKeeper.StateKeeper.JUMP
            : (int)AnimationStateKeeper.StateKeeper.FALL;
        PlayAnimation(AnimationConstants.MOVEMENT, AnimationStateKeeper.CurrentPlayerState);
    }
    public void UpdateJumpTime(string parameterName, float jumpTime)
    {
        PlayAnimation(parameterName, jumpTime);
    }

    public void Sliding(bool keystroke)
    {
        PlayAnimation(AnimationConstants.SLIDING, keystroke);
    }

    public float ReturnCurrentAnimation()
    {
        return _anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }

    public bool IsNameOfTheCurrentAnimation(string name)
    {
        return _anim.GetCurrentAnimatorStateInfo(0).IsName(name);
    }

    public Animator getAnimator()
    {
        return _anim;
    }

    public void OnNotify(GenericState<PlayerState> data, NotificationContext notificationContext, SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken, params object[] optional)
    {
        throw new System.NotImplementedException();
    }
}