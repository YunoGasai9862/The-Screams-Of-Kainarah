using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPlayer : MonoBehaviour
{
   
    [SerializeField] LayerMask player;
    private Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, transform.right * 1f, Color.black);

        if ( Physics2D.Raycast(transform.position, transform.right, 1f, player ))
        {
            EnemyJumping.Attacking = true;
            anim.SetBool("Attack", true);
            anim.SetBool("CanWalk", false);

        }

       
    }
}
