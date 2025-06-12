using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashStateMachine : RakashBaseStateMachine
{
    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private const float MAX_DISTANCE_BETWEEN_PLAYER = 15f;

    private const float MIN_DISTANCE_BETWEEN_PLAYER = 3f;

    protected override void CustomOnStateUpdateLogic(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (GameState.Equals(GameState.DIALOGUE_TAKING_PLACE))
        {
            return;
        }

        if (Player != null && HelperFunctions.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {
            RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.START_WALK, Animator = animator, TargetTransform = Player.Transform });
        }
        else
        {
            RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.STOP_ATTACK, Animator = animator, TargetTransform = Player.Transform });

            RakashAttackCommandController.Execute(new AttackAnimationPackage() { Animation = Animation.START_ATTACK, Animator = animator, AttackDelay = TIME_SPAN_BETWEEN_EACH_ATTACK });
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            RakashAttackCommandController.Execute(new AttackAnimationPackage() { Animation = Animation.START_ATTACK, Animator = animator , AttackDelay = TIME_SPAN_BETWEEN_EACH_ATTACK });
        }
    }
}
