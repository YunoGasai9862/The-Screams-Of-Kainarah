using UnityEngine;
public class MonsterFollow : StateMachineBehaviour
{
    public static GameObject Player;

    public const float TIME_SPAN_BETWEEN_EACH_ATTACK = 0.5f;

    private DialogueTakingPlaceEvent m_dialogueTakingPlaceEvent;
    private bool DialogueTakingPlace { get; set; }

    public void OnEnable()
    {
        m_dialogueTakingPlaceEvent = Helper.GetCustomEvent<DialogueTakingPlaceEvent>();

        m_dialogueTakingPlaceEvent.AddListener(DialogueTakingPlaceListener);
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Player = GameObject.FindGameObjectWithTag("Player");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!DialogueTakingPlace)
        {
            if (Player != null && HelperFunctions.CheckDistance(animator, 15f, 3f, Player))
            {
                animator.SetBool("walk", true);
            }

            if (Vector3.Distance(Player.transform.position, animator.transform.position) <= 3)
            {
                HelperFunctions.DelayAttack(animator, TIME_SPAN_BETWEEN_EACH_ATTACK, "attack");
            }

        }
    }

    private void DialogueTakingPlaceListener(bool isTakingPlace)
    {
        DialogueTakingPlace = isTakingPlace;
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
