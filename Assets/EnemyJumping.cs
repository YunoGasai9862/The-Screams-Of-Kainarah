using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJumping : MonoBehaviour
{

    //ENEMY JUMPING FROM ONE PLATFORM TO ANOTHER
    private Rigidbody2D rb;
    private Animator anim;
    public ArrayList TraversalPoints = new ArrayList();
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        anim=GetComponent<Animator>();
        TraversalPoints.Add(GameObject.FindWithTag("Jump"));
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(20 * Time.deltaTime, 0);
        if (rb.velocity.magnitude > .1f)
        {
            anim.SetBool("CanWalk", true);
        }
    }

}
