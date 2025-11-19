public class PlayerStateBundle : IStateBundle
{
    public State<ActionState, bool> PlayerActionState { get; set; } = new State<ActionState, bool>();

    public State<MovementState, bool> PlayerMovementState { get; set; } = new State<MovementState, bool>();

    public State<AttackState, bool> PlayerAttackState { get; set; } = new State<AttackState, bool>();
    
    public override string ToString()
    {
        return $"PlayerStateBundle - PlayerActionState : {PlayerActionState} - PlayerMovementState : {PlayerMovementState} - PlayerAttackState: {PlayerAttackState}";
    }

    public PlayerStateBundle Clone()
    {
        return new PlayerStateBundle()
        {
            PlayerActionState = this.PlayerActionState,
            PlayerMovementState = this.PlayerMovementState,
            PlayerAttackState = this.PlayerAttackState,
        };
    }
}