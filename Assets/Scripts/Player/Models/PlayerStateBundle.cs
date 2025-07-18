public class PlayerStateBundle : IStateBundle
{
    public State<PlayerActionState> PlayerActionState { get; set; } = new State<PlayerActionState>();

    public State<PlayerMovementState> PlayerMovementState { get; set; } = new State<PlayerMovementState>();

    public State<PlayerAttackState> PlayerAttackState { get; set; } = new State<PlayerAttackState>();
}