using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class RakashMovement : RakashBaseStateMachine
{
    private const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

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
            Vector3 newPos = Player.Transform.position;

            newPos.y = Player.Transform.position.y - 1.5f;

            animator.transform.position = Vector3.MoveTowards(animator.transform.position, newPos, 4f * Time.deltaTime);
        }
        else
        {
            animator.SetBool("walk", false);

            HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
        }
    }
}
