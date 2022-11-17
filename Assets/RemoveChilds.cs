using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class RemoveChilds : MonoBehaviour
{

    private float timing = 0f;
    private TilemapCollider2D tmc2d;
    private float timelapsedforthechild = 0f;

    private void Start()
    {
        tmc2d= GetComponent<TilemapCollider2D>();
    }
    void Update()
    {

        if(transform.childCount>0)
        {
            timelapsedforthechild += Time.deltaTime;
        }

        if(transform.childCount>0 && timelapsedforthechild>.2f)
        {

            tmc2d.enabled = false;
            timelapsedforthechild = 0f;
           
        }

        if(!tmc2d.enabled)
        {
            timing += Time.deltaTime;
        }

        if(timing>.1f)
        {
            tmc2d.enabled = true;
            timing = 0f;
        }
        


    }
}
