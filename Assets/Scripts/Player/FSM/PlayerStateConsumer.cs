using UnityEngine.Events;
using UnityEngine;
using System.Threading.Tasks;

public class PlayerStateConsumer : BaseState<PlayerStateBundle>
{
     public PlayerStateEvent PlayerStateEvent { get; set; }

    protected override async Task AddEvent()
    {
        PlayerStateEvent = await Helper.GetCustomEvent<PlayerStateEvent>();
    }

    protected override UnityEvent<GenericStateBundle<PlayerStateBundle>> GetEvent()
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