using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [SerializeField] GameObject Heroine;
    private Animator anim;
    void Start()
    {
        Heroine = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(CanAttack())
        {
            anim.SetBool("EnemyAttack", true);
        }
        else
        {
            anim.SetBool("EnemyAttack", false);

        }


        if (anim.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttacked"))
        {
            

        }
    }


    bool CanAttack()
    {
        if(Vector2.Distance(transform.position, Heroine.transform.position)<=3.0f)
        {
            return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            anim.SetBool("Hit", true);

        }
    }
  
}
