using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
   
    [SerializeField] LayerMask player;
    private Animator anim;
    private int multiplier = -1;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (GetComponent<SpriteRenderer>().flipX == false)
        {
            multiplier=1;
        }
        else
        {
            multiplier =-1;

        }

        Debug.DrawRay(transform.position, (multiplier)*transform.right * 1f, Color.black);

        if (Physics2D.Raycast(transform.position, (multiplier) * transform.right, 1f, player))
        {
            EnemyJumping.Attacking = true;
            if(GetComponent<SpriteRenderer>().flipX)
            {
                anim.SetBool("AttackL", true);

            }else
            {
                anim.SetBool("Attack", true);

            }
            anim.SetBool("CanWalk", false);

        }
        else
        {
            EnemyJumping.Attacking = false;

            if (GetComponent<SpriteRenderer>().flipX)
            {
                anim.SetBool("AttackL", false);

            }
            else
            {
                anim.SetBool("Attack", false);

            }
            anim.SetBool("CanWalk", true);
        }


    }
}
