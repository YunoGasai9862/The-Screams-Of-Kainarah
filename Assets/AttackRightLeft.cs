using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackRightLeft : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer sr;
    [SerializeField] LayerMask Player;
    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {



        if(sr.flipX && Physics2D.Raycast(transform.position, -transform.right, 3f, Player))
        {
            anim.SetBool("AL", true);
            anim.SetBool("AR", false);

        }else if(!sr.flipX && Physics2D.Raycast(transform.position, transform.right, 3f, Player))
        {
            anim.SetBool("AR", true);
            anim.SetBool("AL", false);

        }
    }
}
