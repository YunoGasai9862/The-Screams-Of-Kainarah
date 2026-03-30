using Annotations.Enums;
using Assets.Annotations;
using UnityEngine;

[Subject(AssetType = Asset.MONOBEHAVIOR, SubjectType = typeof(PlayerStateConsumer), ContextType = typeof(GenericStateBundle<PlayerStateBundle>))]
public class PlayerStateConsumer : BaseState<PlayerStateBundle>
{
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