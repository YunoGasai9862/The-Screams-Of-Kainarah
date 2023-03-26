using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingPlayer : MonoBehaviour
{
    private GameObject Player;
    private Animator anim;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if(!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_02"))
        {
            if (transform.position.x > Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if ( transform.position.x < Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }

        
    }
}
