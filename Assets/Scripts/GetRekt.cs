using UnityEngine;

public class GetRekt : MonoBehaviour
{
    private int HitCount = 0;
    private Animator anim;
    private int SwordHit = 0;
    [SerializeField] GameObject HitEffect;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (HitCount >= 3 || SwordHit >= 3)
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

    private void HitEffectFun()
    {
        GameObject _hitEffect = Instantiate(HitEffect, transform.position, HitEffect.transform.rotation);
        Destroy(_hitEffect, 1f);

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Dagger"))
        {
            HitEffectFun();
            anim.SetBool("Hit", false);

        }

        if (collision.CompareTag("Sword"))
        {
            HitEffectFun();

            anim.SetBool("Hit", false);

        }

    }
}
