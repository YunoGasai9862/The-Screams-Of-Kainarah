public class PlayerStateBundle : IStateBundle
{
    public State<ActionState> PlayerActionState { get; set; } = new State<ActionState>();

    public State<MovementState> PlayerMovementState { get; set; } = new State<MovementState>();

    public State<AttackState> PlayerAttackState { get; set; } = new State<AttackState>();
    
    public override string ToString()
    {
        return $"PlayerStateBundle - PlayerActionState : {PlayerActionState} - PlayerMovementState : {PlayerMovementState} - PlayerAttackState: {PlayerAttackState}";
    }
}