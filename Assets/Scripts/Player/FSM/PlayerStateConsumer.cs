using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerStateConsumer : BaseState<PlayerStateBundle>
{
     public PlayerStateDelegator PlayerStateDelegator { get; set; }

     public PlayerStateEvent PlayerStateEvent { get; set; }

    protected override async void AddSubject()
    {
        PlayerStateDelegator = await Helper.GetDelegator<PlayerStateDelegator>();

        PlayerStateDelegator.AddToSubjectsDict(typeof(PlayerStateConsumer).ToString(), gameObject.name, new Subject<IObserver<GenericStateBundle<PlayerStateBundle>>>());

        PlayerStateDelegator.GetSubsetSubjectsDictionary(typeof(PlayerStateConsumer).ToString())[gameObject.name].SetSubject(this);
    }

    protected override BaseDelegator<GenericStateBundle<PlayerStateBundle>> GetDelegator()
    {
        return PlayerStateDelegator;
    }

    protected override async Task<UnityEvent<GenericStateBundle<PlayerStateBundle>>> GetEvent()
    {
        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();

        return PlayerStateEvent.GetInstance();
    }

    protected override GenericStateBundle<PlayerStateBundle> GetInitialState()
    {
        return new GenericStateBundle<PlayerStateBundle>()
        {
            StateBundle = new PlayerStateBundle()
            {
                PlayerActionState = new State<PlayerActionState>()
                {
                    CurrentState = PlayerActionState.IDLE
                },
                PlayerAttackState = new State<PlayerAttackState>()
                {
                    CurrentState = PlayerAttackState.IDLE,
                },
                PlayerMovementState = new State<PlayerMovementState>()
                {
                    CurrentState = PlayerMovementState.IS_IDLE,
                }
            }
        };
    }
}