using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LedgeDetector : MonoBehaviour
{
    [SerializeField] LayerMask Ledge;
    private BoxCollider2D _col;
    public bool Allowed;

    // Update is called once per frame
    void Update()
    {
        if(CheckForLedge())
        {
            Allowed = true;
        }else
        {
            Allowed = false;
        }
        Debug.DrawRay(transform.position, transform.up * .3f, Color.black);
    }


    bool CheckForLedge()
    {
      
        return Physics2D.Raycast(transform.position, transform.up, .3f, Ledge); 
    }
}
