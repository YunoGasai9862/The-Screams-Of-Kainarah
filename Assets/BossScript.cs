using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : MonoBehaviour
{
    // Start is called before the first frame update

    public static float MAXHEALTH = 100;

    private GameObject Player;
    private Animator anim;
    private float TimeoverBody = 0f;
    private BoxCollider2D _bC2;
    private bool onTopBossBool = false;
    [SerializeField] GameObject BossDead;
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        anim = GetComponent<Animator>();
        MAXHEALTH = 100;
        _bC2= GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckRotation();

 
         if(onTopBossBool)
        {
            TimeoverBody += Time.deltaTime;
        }

         if(TimeoverBody>.5f)
        {
            _bC2.enabled = false;
            onTopBossBool = false;
            StartCoroutine(TimeElapse());

        }






    }

    IEnumerator TimeElapse()
    {
        yield return new WaitForSeconds(.5f);
        _bC2.enabled = true;
        TimeoverBody = 0f;

    }

    void CheckRotation()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attack") && !anim.GetCurrentAnimatorStateInfo(0).IsName("attack_02"))
        {
            if (transform.position.x > Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            if (transform.position.x < Player.transform.position.x)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Sword"))
        {
            anim.SetTrigger("damage");
            MAXHEALTH -= 10;

        }

        if(MAXHEALTH==0)
        {
            GameObject dead = Instantiate(BossDead, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(dead, 1f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player") && onTopBossBool==false)
        {
         
            onTopBossBool = true;

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            onTopBossBool = false;

        }

    }

   

}
