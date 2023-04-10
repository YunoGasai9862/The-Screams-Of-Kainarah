using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowPlayerMonster : StateMachineBehaviour
{
    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state

    
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    //OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        if (!DialogueManager.IsOpen)
        {

            if (MonsterFollow.Player != null && MonsterFollow.checkDistance(animator))
            {



            //   float crossProduct = Vector3.Dot(animator.transform.position, MonsterFollow.Player.transform.position);
            // crossProduct = crossProduct / (animator.transform.position.magnitude * MonsterFollow.Player.transform.position.magnitude);


            //converting to degrees
            // double angleinDegrees = crossProduct * (180.0f / (Math.PI));


            // animator.transform.rotation = Quaternion.AngleAxis((float)angleinDegrees, Vector3.up);

                 
                        Vector3 newPos = MonsterFollow.Player.transform.position;
                        newPos.y = MonsterFollow.Player.transform.position.y - 1.5f;

                        animator.transform.position = Vector3.MoveTowards(animator.transform.position, newPos, 4f * Time.deltaTime);

                    

                 
            }


        
         
           

        

                if(!MonsterFollow.checkDistance(animator))
                {

                    animator.SetBool("walk", false);
                    MonsterFollow.DelayAttack(animator);
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
}
