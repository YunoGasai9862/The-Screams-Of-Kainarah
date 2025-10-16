using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerStateConsumer : BaseState<PlayerStateBundle>
{
     public PlayerStateDelegator PlayerStateDelegator { get; set; }

     public PlayerStateEvent PlayerStateEvent { get; set; }

    protected override async Task AddDelegator()
    {
        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();
    }

    protected override async Task AddEvent()
    {
        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();
    }

    protected override async Task AddSubject()
    {
        PlayerStateDelegator.AddToSubjectsDict(typeof(PlayerStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<PlayerStateBundle>>>());

        PlayerStateDelegator.GetSubsetSubjectsDictionary(typeof(PlayerStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override async Task<BaseDelegator<GenericStateBundle<PlayerStateBundle>>> GetDelegator()
    {
        return PlayerStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<PlayerStateBundle>>> GetEvent()
    {
        return PlayerStateEvent.GetInstance();
    }

    protected override GenericStateBundle<PlayerStateBundle> GetInitialState()
    {
        return new GenericStateBundle<PlayerStateBundle>()
        {
            StateBundle = new PlayerStateBundle()
            {
                PlayerActionState = new State<ActionState>()
                {
                    CurrentState = ActionState.IDLE
                },
                PlayerAttackState = new State<AttackState>()
                {
                    CurrentState = AttackState.IDLE,
                },
                PlayerMovementState = new State<MovementState>()
                {
                    CurrentState = MovementState.IS_IDLE,
                }
            }
        };
    }
}