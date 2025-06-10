using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashFollow : RakashBaseStateMachine
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
            RakashMovementCommandController.Execute(new RakashAnimationPackage() { RakashAnimation = RakashAnimation.START_WALK, RakashAnimator = animator });
        }

        if (Vector3.Distance(Player.Transform.position, animator.transform.position) <= MIN_DISTANCE_BETWEEN_PLAYER)
        {
            RakashAttackCommandController.Execute(new RakashAnimationPackage() { RakashAnimation = RakashAnimation.START_ATTACK, RakashAnimator = animator });

            HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
        }
    }
}
