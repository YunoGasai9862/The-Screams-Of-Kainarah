using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRightLeft : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] LayerMask Player;
    private bool StopForAtack = false;
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {

        if(sr.flipX)
        {
            if (Physics2D.Raycast(transform.position, -transform.right, 3f, Player))
            {

                anim.SetBool("AL", true);
            }
            else
            {
                anim.SetBool("AL", false);

            }


        }
        else
        {

            if (Physics2D.Raycast(transform.position, transform.right, 3f, Player))
            {
                anim.SetBool("AR", true);

            }
            else
            {
                anim.SetBool("AR", false);

            }
        }

       
            
        
         
    }
}
