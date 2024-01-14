using UnityEngine;
public class MonsterFollow : StateMachineBehaviour


{


    public static GameObject Player;
    static float timeSpanbetweenEachAttack = 0f;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!GameObjectCreator.GetDialogueManager().IsOpen())
        {
            if (Player != null && checkDistance(animator))
            {

                animator.SetBool("walk", true);

            }


            if (Vector3.Distance(Player.transform.position, animator.transform.position) <= 3)
            {
                DelayAttack(animator);

            }


        }



    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

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

    public static bool checkDistance(Animator animator)
    {
        return Vector3.Distance(Player.transform.position, animator.transform.position) <= 15f && Vector3.Distance(Player.transform.position, animator.transform.position) >= 3;
    }

    public static void DelayAttack(Animator animator)
    {
        timeSpanbetweenEachAttack += Time.deltaTime;
        if (timeSpanbetweenEachAttack > 1f)
        {
            animator.SetTrigger("attack");
            timeSpanbetweenEachAttack = 0f;

        }
    }



}
