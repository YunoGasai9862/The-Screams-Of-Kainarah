using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGenerator : MonoBehaviour
{
    [SerializeField] GameObject SmokeFlare;
    [SerializeField] int NumberOfFlares;
    private Vector2 FlarePos;
    void Start()
    {
        FlarePos = transform.position;

        for(int i=0; i<NumberOfFlares; i++)
        {
            FlarePos = new Vector3(transform.position.x - (i * 8), transform.position.y);
            Instantiate(SmokeFlare, FlarePos, Quaternion.identity);
        }
        
    }

   
}
