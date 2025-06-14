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
            animator.transform.position = RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.STOP_WALK, Animator = animator });

            return;
        }

        if (Player != null && Helper.CheckDistance(animator.transform, Player.Transform, MAX_DISTANCE_BETWEEN_PLAYER, MIN_DISTANCE_BETWEEN_PLAYER))
        {
           animator.transform.position = RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.START_WALK, Animator = animator, TargetTransform = Player.Transform });
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            animator.transform.position = RakashMovementCommandController.Execute(new MovementAnimationPackage() { Animation = Animation.STOP_WALK, Animator = animator });

            RakashAttackCommandController.Execute(new AttackAnimationPackage() { Animation = Animation.START_ATTACK, Animator = animator , AttackDelay = TIME_SPAN_BETWEEN_EACH_ATTACK });
        }
    }
}
