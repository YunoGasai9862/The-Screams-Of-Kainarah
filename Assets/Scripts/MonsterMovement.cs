using UnityEngine;
public class MonsterMovement : StateMachineBehaviour
{
    private const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!SceneSingleton.IsDialogueTakingPlace)
        {
            if (MonsterFollow.Player != null && HelperFunctions.CheckDistance(animator, 15f, 3f, MonsterFollow.Player))
            {
                Vector3 newPos = MonsterFollow.Player.transform.position;
                newPos.y = MonsterFollow.Player.transform.position.y - 1.5f;

                animator.transform.position = Vector3.MoveTowards(animator.transform.position, newPos, 4f * Time.deltaTime);
            }
            else
            {
                animator.SetBool("walk", false);
                HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
            }

        }

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
