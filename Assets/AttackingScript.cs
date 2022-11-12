using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.UIElements;

public class AttackingScript : MonoBehaviour
{
    private Animator anim;
    private float elapsedTime = 0;
    private bool kickoffElapsedTime;
    private int AttackCount = 0;
    private BoxCollider2D col;
    private GameObject dag;
    private bool canthrowDagger = true;
    private float throwdaggerTime=0;
    [SerializeField] LayerMask Ground;
    [SerializeField] LayerMask ledge;
    [SerializeField] GameObject Dagger;

    void Start()
    {
        anim = GetComponent<Animator>();
        col = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        if (kickoffElapsedTime)
        {
            elapsedTime += Time.deltaTime;
        }

        AttackingMechanism();

    }

    void AttackingMechanism()
    {

        if (!isOntheGround() && Input.GetMouseButtonDown(0))
        {
            anim.SetBool("AttackJ", true);
        }
        else
        {
            anim.SetBool("AttackJ", false);

        }

        if ((isOntheGround() || isontheLedge()) && Input.GetMouseButtonDown(0))
        {
            kickoffElapsedTime = true;

            AttackCount++;
            anim.SetInteger("AttackCount", AttackCount);

            anim.SetBool("Attack", true);
            elapsedTime = 0;  // YAYAY SOLVED IT!!!

        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            //fix with new elapsedTime thingy

            anim.SetFloat("ElapsedTime", elapsedTime);

            if (elapsedTime>.5f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }


        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }



        }
        else if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack4"))
        {
            anim.SetFloat("ElapsedTime", elapsedTime);
            if (elapsedTime > .8f)
            {

                AttackCount = 0;
                elapsedTime = 0;
                anim.SetBool("Attack", false);
                kickoffElapsedTime = false;
            }

        }

        if (AttackCount > 4)
        {
            anim.SetBool("Attack", false);
            AttackCount = 0;

        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("ThrowDagger"))
        {
            throwdaggerTime += Time.deltaTime;
            if (throwdaggerTime >.5f)
            {
                anim.SetBool("ThrowDagger", false);
                throwdaggerTime = 0f;
                canthrowDagger = true;

            }




        }


        if (!(anim.GetCurrentAnimatorStateInfo(0).IsName("Running")) && Input.GetKeyDown(KeyCode.F) && canthrowDagger)
            {
                    anim.SetBool("ThrowDagger", true);
                    canthrowDagger = false;


                    Invoke("instantiateDag", .4f);

              }



        bool isOntheGround()
        {
            return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, Ground);
        }

        bool isontheLedge()
        {
            return Physics2D.BoxCast(col.bounds.center, col.bounds.size, 0f, Vector2.down, .1f, ledge);
        }
    }

    void instantiateDag()
    {
        Vector3 position = transform.position;
        position.y = transform.position.y - 1f;

        dag = Instantiate(Dagger, position, Quaternion.identity);

    }
}
