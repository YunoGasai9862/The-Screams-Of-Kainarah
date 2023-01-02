using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetRekt : MonoBehaviour
{
    private int HitCount = 0;
    private Animator anim;
    private int SwordHit = 0;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (HitCount >= 3 || SwordHit>=3)
        {
            Destroy(gameObject, 1f);
            HitCount = 0;
            SwordHit = 0;
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            HitCount++;
            anim.SetBool("Hit", true);
        }

        if (collision.CompareTag("Sword"))
        {
            SwordHit++;
            anim.SetBool("Hit", true);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            anim.SetBool("Hit", false);

        }

        if (collision.CompareTag("Sword"))
        {
            anim.SetBool("Hit", false);

        }

    }
}
