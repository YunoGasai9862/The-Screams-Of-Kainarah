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
        PlayerStateDelegator.AddToSubjectsDict(typeof(PlayerStateConsumer).ToString(), gameObject.name, new Subject<GenericStateBundle<PlayerStateBundle>>());

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
                PlayerActionState = new State<ActionState, bool>()
                {
                    CurrentState = ActionState.IDLE,
                    CurrentValue = true
                },
                PlayerAttackState = new State<AttackState, bool>()
                {
                    CurrentState = AttackState.IDLE,
                    CurrentValue = true
                },
                PlayerMovementState = new State<MovementState, MovementDto>()
                {
                    CurrentState = MovementState.IS_IDLE,
                    CurrentValue = new MovementDto()
                    {
                        CharacterSpeed = Vector2.zero
                    }
                }
            }
        };
    }
}